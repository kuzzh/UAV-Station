using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Reflection;
using log4net;
using MAVLink;


namespace SanHeGroundStation.Forms.ProgressReporterSphereUsing
{
    public class MagCalib
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        const float rad2deg = (float) (180/Math.PI);
        const float deg2rad = (float) (1.0/rad2deg);

        static double error = 99;
        static double error2 = 99;
        static double[] ans;
        static double[] ans2;

        static string GetColour(int pitch, int yaw)
        {
            // yaw doesnt matter with these 2
            if (pitch == 0)
                return "DarkBlue";

            if (pitch == 180)
                return "Yellow";

            // select hemisphere
            if (pitch < 90)
            {
                if (yaw < 90 || yaw > 270)
                    return "DarkBlue-Red";
                if (yaw < 180)
                    return "DarkBlue-Blue";
                if (yaw < 270)
                    return "DarkBlue-Pink";
            }
            else
            {
                if (yaw < 90 || yaw > 270)
                    return "Yellow-Green";
                if (yaw < 180)
                    return "Yellow-Blue";
                if (yaw < 270)
                    return "Yellow-Pink";
            }

            return "";
        }


        public static void DoGUIMagCalib(bool dointro = true)
        {
            ans = null;
            filtercompass1.Clear();
            datacompass1.Clear();
            datacompass2.Clear();
            filtercompass2.Clear();
            error = 99;
            error2 = 99;

            if (dointro)
                MessageBox.Show("该功能目前正在测试中！");

            using (ProgressReporterSphere prd = new ProgressReporterSphere())
            {
                
                prd.DoWork += prd_DoWork;

                prd.RunBackgroundOperationAsync();

                
            }

            if (ans != null)
                MagCalib.SaveOffsets(ans);

            if (ans2 != null)
                MagCalib.SaveOffsets2(ans2);

        }

        // filter data points to only x number per quadrant
        public static int div = 20;
        public static Hashtable filtercompass1 = new Hashtable();
        public static Hashtable filtercompass2 = new Hashtable();

        // list of x,y,z 's
        public static List<Tuple<float, float, float>> datacompass1 = new List<Tuple<float, float, float>>();
        // list no 2
        public static List<Tuple<float, float, float>> datacompass2 = new List<Tuple<float, float, float>>();
        
        public static bool boostart = false;

        public static Tuple<float, float, float> comdatacompass1 = null;
        public static Tuple<float, float, float> comdatacompass2 = null;

        
        static void prd_DoWork(object sender, ProgressWorkerEventArgs e, object passdata = null)
        {
           

            bool havecompass2 = false;
            havecompass2 = true;

            int hittarget = 14; 


            // old method
            float minx = 0;
            float maxx = 0;
            float miny = 0;
            float maxy = 0;
            float minz = 0;
            float maxz = 0;

            MavLinkInterface mavLinkInterface = new MavLinkInterface();
            MavLink.mavlink_request_data_stream_t req = new MavLink.mavlink_request_data_stream_t();
           
            mavLinkInterface = new MavLinkInterface();
            req = new MavLink.mavlink_request_data_stream_t();
            req.target_system = Global.sysID;
            req.target_component = Global.compID;
            req.req_message_rate = 30; //50
            req.start_stop = 1; // start
            req.req_stream_id = (byte)1; // id 1
            // send each one twice.
            mavLinkInterface.AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.REQUEST_DATA_STREAM, req);
            mavLinkInterface.AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.REQUEST_DATA_STREAM, req);


            string extramsg = "";

            // clear any old data
            ((ProgressReporterSphere)sender).sphere1.Clear();
            ((ProgressReporterSphere)sender).sphere2.Clear();

            // keep track of data count and last lsq run
            int lastcount = 0;
            DateTime lastlsq = DateTime.MinValue;
            DateTime lastlsq2 = DateTime.MinValue;
            DateTime lastlsq3 = DateTime.MinValue;

            HI.Vector3 centre = new HI.Vector3();

            int pointshitnum = 0;

            while (true)
            {
                // slow down execution
                System.Threading.Thread.Sleep(10);

                string str = "  共有 " + datacompass1.Count + " 份数据\n\n" +
                             "  罗盘 1 误差：" + error + "\n" +
                             "  罗盘 2 误差：" + error2 + "\n" +
                             "  结束条件，罗盘 1 误差不大于0.2";
                str += "\n  " + extramsg;

                ((ProgressReporterDialogue)sender).UpdateProgressAndStatus(-1, str);

                addcompass1list();
                addcompass2list();

               
                if (e.CancelRequested)
                {
                    e.CancelAcknowledged = false;
                    e.CancelRequested = false;
                    break;
                }

                if (datacompass1.Count == 0)
                    continue;

                float rawmx = datacompass1[datacompass1.Count - 1].Item1;
                float rawmy = datacompass1[datacompass1.Count - 1].Item2;
                float rawmz = datacompass1[datacompass1.Count - 1].Item3;

                // for old method
                setMinorMax(rawmx, ref minx, ref maxx);
                setMinorMax(rawmy, ref miny, ref maxy);
                setMinorMax(rawmz, ref minz, ref maxz);


                if ( datacompass1.Count > 100 && lastlsq.Second != DateTime.Now.Second ) //w
                {
                    //req = new MavLink.mavlink_request_data_stream_t();
                    //req.target_system = Global.sysID;
                    //req.target_component = Global.compID;
                    //req.req_message_rate = 30; //50
                    //req.start_stop = 1; // start
                    //req.req_stream_id = (byte)1; // id 1
                    //// send each one twice.
                    //mavLinkInterface.AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.REQUEST_DATA_STREAM, req);
                    //mavLinkInterface.AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.REQUEST_DATA_STREAM, req);


                    lastlsq = DateTime.Now;
                    lock (datacompass1)
                    {
                        var lsq = MagCalib.LeastSq(datacompass1, false);
                        // simple validation
                        if (Math.Abs(lsq[0]) < 999)
                        {
                            centre = new HI.Vector3(lsq[0], lsq[1], lsq[2]);

                            ((ProgressReporterSphere)sender).sphere1.CenterPoint = new OpenTK.Vector3(
                                (float)centre.x, (float)centre.y, (float)centre.z);
                        }
                    }
                }

                // run lsq every second when more than 100 datapoints
                if (datacompass2.Count > 100 && lastlsq2.Second != DateTime.Now.Second)//w
                {
                    lastlsq2 = DateTime.Now;
                    lock (datacompass2)
                    {
                        var lsq = MagCalib.LeastSq(datacompass2, false);
                        // simple validation
                        if (Math.Abs(lsq[0]) < 999)
                        {
                            HI.Vector3 centre2 = new HI.Vector3(lsq[0], lsq[1], lsq[2]);

                            ((ProgressReporterSphere)sender).sphere2.CenterPoint = new OpenTK.Vector3(
                                (float)centre2.x, (float)centre2.y, (float)centre2.z);
                        }
                    }
                }

                if (lastcount == datacompass1.Count)
                    continue;

                lastcount = datacompass1.Count;

                // add to sphere with center correction
                ((ProgressReporterSphere)sender).sphere1.AddPoint(new OpenTK.Vector3(rawmx, rawmy, rawmz));
                ((ProgressReporterSphere)sender).sphere1.AimClear();

                if (datacompass2.Count > 30)//w
                {
                    float raw2mx = datacompass2[datacompass2.Count - 1].Item1;
                    float raw2my = datacompass2[datacompass2.Count - 1].Item2;
                    float raw2mz = datacompass2[datacompass2.Count - 1].Item3;

                    ((ProgressReporterSphere)sender).sphere2.AddPoint(new OpenTK.Vector3(raw2mx, raw2my, raw2mz));
                    ((ProgressReporterSphere)sender).sphere2.AimClear();
                }

                HI.Vector3 point;

                point = new HI.Vector3(rawmx, rawmy, rawmz) + centre;

                //find the mean radius                    
                float radius = 0;
                for (int i = 0; i < datacompass1.Count; i++)
                {
                    point = new HI.Vector3(datacompass1[i].Item1, datacompass1[i].Item2, datacompass1[i].Item3);
                    radius += (float)(point + centre).length();
                }
                radius /= datacompass1.Count;

                //test that we can find one point near a set of points all around the sphere surface
                int pointshit = 0;
                string displayresult = "";
                int factor = 3; // pitch
                int factor2 = 4; // yaw
                float max_distance = radius / 3; //pretty generouse
                for (int j = 0; j <= factor; j++)
                {
                    double theta = (Math.PI * (j + 0.5)) / factor;

                    for (int i = 0; i <= factor2; i++)
                    {
                        double phi = (2 * Math.PI * i) / factor2;

                        HI.Vector3 point_sphere = new HI.Vector3(
                            (float)(Math.Sin(theta) * Math.Cos(phi) * radius),
                            (float)(Math.Sin(theta) * Math.Sin(phi) * radius),
                            (float)(Math.Cos(theta) * radius)) - centre;

                        //log.InfoFormat("magcalib check - {0} {1} dist {2}", theta * rad2deg, phi * rad2deg, max_distance);

                        bool found = false;
                        for (int k = 0; k < datacompass1.Count; k++)
                        {
                            point = new HI.Vector3(datacompass1[k].Item1, datacompass1[k].Item2, datacompass1[k].Item3);
                            double d = (point_sphere - point).length();
                            if (d < max_distance)
                            {
                                pointshit++;
                                found = true;
                                break;
                            }
                        }
                  

                        // draw them all
                        //((ProgressReporterSphere)sender).sphere1.AimFor(new OpenTK.Vector3((float)point_sphere.x, (float)point_sphere.y, (float)point_sphere.z));
                        //if (!found)
                        //{
                        //    displayresult = "需要更多的数据 " +
                        //                    GetColour((int)(theta * rad2deg), (int)(phi * rad2deg));
                        //    ((ProgressReporterSphere)sender).sphere1.AimFor(new OpenTK.Vector3((float)point_sphere.x,
                        //        (float)point_sphere.y, (float)point_sphere.z));
                        //    //j = factor;
                        //    //break;
                        //}
                    }
                }

                extramsg = displayresult;



                if (error < 0.2 && pointshit > hittarget && ((ProgressReporterSphere)sender).autoaccept)
                { 
                    extramsg = "";
                    MessageBox.Show("旋转完成\r\n请自行关闭罗盘校准窗口");
                    break;
                }

            }

            req = new MavLink.mavlink_request_data_stream_t();
            req.target_system = Global.sysID;
            req.target_component = Global.compID;
            req.req_message_rate = 2; //50
            req.start_stop = 1; // start
            req.req_stream_id = (byte)1; // id 1
            // send each one twice.
            mavLinkInterface.AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.REQUEST_DATA_STREAM, req);
            mavLinkInterface.AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.REQUEST_DATA_STREAM, req);

            ans = MagCalib.LeastSq(datacompass1, false);
            ans2 = MagCalib.LeastSq(datacompass2, false);

            

        }

        private static void addcompass1list()
        {
            if (comdatacompass1 == null) return;
            string item = (int)(comdatacompass1.Item1 / div) + "," + (int)(comdatacompass1.Item2 / div) + "," + (int)(comdatacompass1.Item3 / div);

            if ( filtercompass1.ContainsKey(item))
            {
                filtercompass1[item] =(int)filtercompass1[item] + 1;

                if ((int)filtercompass1[item] > 3)
                {
                    return;
                }

            }
            else
            {
                filtercompass1[item] = 1;
            }

            datacompass1.Add(comdatacompass1);
          
        }

        private static void addcompass2list()
        {
            if (comdatacompass2 == null) return;

            string item = (int)(comdatacompass2.Item1 / div) + "," + (int)(comdatacompass2.Item2 / div) + "," + (int)(comdatacompass2.Item3 / div);

            if ( filtercompass2.ContainsKey(item))
            {
                filtercompass2[item] =(int)filtercompass2[item] + 1;

                if ((int)filtercompass2[item] > 3)
                {
                    return;
                }

            }
            else
            {
                filtercompass2[item] = 1;
            }

            datacompass2.Add(comdatacompass1);
          
        }


        static double avg_samples = 0;

        /// <summary>
        /// Does the least sq adjustment to find the center of the sphere
        /// </summary>
        /// <param name="data">list of x,y,z data</param>
        /// <returns>offsets</returns>
        public static double[] LeastSq(List<Tuple<float, float, float>> data, bool ellipsoid = false)
        {
            avg_samples = 0;
            foreach (var item in data)
            {
                avg_samples += Math.Sqrt(Math.Pow(item.Item1, 2) + Math.Pow(item.Item2, 2) + Math.Pow(item.Item3, 2));
            }

            avg_samples /= data.Count;

            log.Info("lsq avg " + avg_samples + " count " + data.Count);

            double[] x;

            //
            x = new double[] { 0, 0, 0, 0 };

            x = doLSQ(data, sphere_error, x);

            rad = x[3];

            log.Info("lsq rad " + rad);

            if (ellipsoid)
            {
                // offsets + diagonals
                x = new double[] { x[0], x[1], x[2], 1, 1, 1 };

                x = doLSQ(data, sphere_ellipsoid_error, x);

                // offsets + diagonals + offdiagonals
                x = new double[] { x[0], x[1], x[2], x[3], x[4], x[5], 0, 0, 0 };

                x = doLSQ(data, sphere_ellipsoid_error, x);
            }

            return x;
        }

        static double[] doLSQ(List<Tuple<float, float, float>> data, Action<double[], double[], object> fitalgo,
            double[] x)
        {
            double epsg = 0.00000001;
            double epsf = 0;
            double epsx = 0;
            int maxits = 0;

            alglib.minlmstate state;
            alglib.minlmreport rep;

            alglib.minlmcreatev(data.Count, x, 100, out state);
            alglib.minlmsetcond(state, epsg, epsf, epsx, maxits);

            var t1 = new alglib.ndimensional_fvec(fitalgo);

            alglib.minlmoptimize(state, t1, null, data);

            alglib.minlmresults(state, out x, out rep);

            log.InfoFormat("passes {0}", rep.iterationscount);
            log.InfoFormat("term type {0}", rep.terminationtype);
            log.InfoFormat("njac {0}", rep.njac);
            log.InfoFormat("ncholesky {0}", rep.ncholesky);
            log.InfoFormat("nfunc{0}", rep.nfunc);
            log.InfoFormat("ngrad {0}", rep.ngrad);
            log.InfoFormat("ans {0}", alglib.ap.format(x, 4));

            if (data == datacompass1)
            {
                error = 0;

                foreach (var item in state.fi)
                {
                    error += item;
                }

                error = Math.Round(Math.Sqrt(Math.Abs(error)), 2);
            }


            if (data == datacompass2)
            {
                error2 = 0;

                foreach (var item in state.fi)
                {
                    error2 += item;
                }

                error2 = Math.Round(Math.Sqrt(Math.Abs(error2)), 2);
            }

            return x;
        }

        /// <summary>
        /// saves the offests to eeprom, os displays if cant
        /// </summary>
        /// <param name="ofs">offsets</param>
        public static void SaveOffsets(double[] ofs)
        {
            MavLinkInterface mavLinkInterface = new MavLinkInterface();
            MavLink.mavlink_command_long_t req = new MavLink.mavlink_command_long_t();
            req.target_system = Global.sysID;
            req.target_component = Global.compID;

            req.command = (ushort)242;

            req.param1 = 2;
            req.param2 = (float)ofs[0];
            req.param3 = (float)ofs[1];
            req.param4 = (float)ofs[2];
            req.param5 = 0;
            req.param6 = 0;
            req.param7 = 0;

           
            // send each one twice.
            mavLinkInterface.AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.REQUEST_DATA_STREAM, req);
            mavLinkInterface.AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.REQUEST_DATA_STREAM, req);

               
            MessageBox.Show(
                    "New offsets for compass #1 are " + ofs[0].ToString("0") + " " + ofs[1].ToString("0") + " " +
                    ofs[2].ToString("0") + "\nThese have been saved for you.", "New Mag Offsets");
        
        }

        public static void SaveOffsets2(double[] ofs)
        {
            MavLinkInterface mavLinkInterface = new MavLinkInterface();
            MavLink.mavlink_command_long_t req = new MavLink.mavlink_command_long_t();
            req.target_system = Global.sysID;
            req.target_component = Global.compID;

            req.command = (ushort)242;

            req.param1 = 5;
            req.param2 = (float)ofs[0];
            req.param3 = (float)ofs[1];
            req.param4 = (float)ofs[2];
            req.param5 = 0;
            req.param6 = 0;
            req.param7 = 0;


            // send each one twice.
            mavLinkInterface.AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.REQUEST_DATA_STREAM, req);
            mavLinkInterface.AssembleAndSendFrame((byte)MavLink.MAVLINK_MSG_ID.REQUEST_DATA_STREAM, req);


            MessageBox.Show(
                    "New offsets for compass #1 are " + ofs[0].ToString("0") + " " + ofs[1].ToString("0") + " " +
                    ofs[2].ToString("0") + "\nThese have been saved for you.", "New Mag Offsets");
        }

       
        /// <summary>
        /// Min or max finder
        /// </summary>
        /// <param name="value">value to process</param>
        /// <param name="min">current min</param>
        /// <param name="max">current max</param>
        private static void setMinorMax(float value, ref float min, ref float max)
        {
            if (value > max)
                max = value;
            if (value < min)
                min = value;
        }

        static double rad = 0;

        static void sphere_ellipsoid_error(double[] p1, double[] fi, object obj)
        {
            var offsets = new HI.Vector3(p1[0], p1[1], p1[2]);
            var diagonals = new HI.Vector3(1.0, 1.0, 1.0);
            var offdiagonals = new HI.Vector3(0.0, 0.0, 0.0);
            if (p1.Length >= 6)
                diagonals = new HI.Vector3(p1[3], p1[4], p1[5]);
            if (p1.Length >= 8)
                offdiagonals = new HI.Vector3(p1[6], p1[7], p1[8]);

            diagonals.x = 1.0;

            int a = 0;
            foreach (var d in (List<Tuple<float, float, float>>)obj)
            {
                var mag = new HI.Vector3(d.Item1, d.Item2, d.Item3);
                double err = rad - radius(mag, offsets, diagonals, offdiagonals);
                fi[a] = err;
                a++;
            }
        }

        static double radius(HI.Vector3 mag, HI.Vector3 offsets, HI.Vector3 diagonals, HI.Vector3 offdiagonals)
        {
            //'''return radius give data point and offsets'''
            HI.Vector3 mag2 = mag + offsets;
            var rot = new HI.Matrix3(new HI.Vector3(diagonals.x, offdiagonals.x, offdiagonals.y),
                new HI.Vector3(offdiagonals.x, diagonals.y, offdiagonals.z),
                new HI.Vector3(offdiagonals.y, offdiagonals.z, diagonals.z));
            mag2 = rot * mag2;
            return mag2.length();
        }


        static void sphere_error(double[] xi, double[] fi, object obj)
        {
            double xofs = xi[0];
            double yofs = xi[1];
            double zofs = xi[2];
            double r = xi[3];
            int a = 0;
            foreach (var d in (List<Tuple<float, float, float>>)obj)
            {
                double x = d.Item1;
                double y = d.Item2;
                double z = d.Item3;
                double err = r - Math.Sqrt(Math.Pow((x + xofs), 2) + Math.Pow((y + yofs), 2) + Math.Pow((z + zofs), 2));
                fi[a] = err;
                a++;
            }
        }

     
    }
}