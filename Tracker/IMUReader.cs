using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Tracker
{
    public class IMUReader
    {
        private UdpClient m_client;
        public IMUReader()
        {
            m_client = new UdpClient(5555);
            m_client.BeginReceive(new AsyncCallback(ReceiveDatagram), this);
        }        

        public Vector3D Acceleration = new Vector3D(0, 0, 0);
        public Vector3D LinearAcceleration = new Vector3D(0, 0, 0);
        public Vector3D LinearVelocity = new Vector3D(0, 0, 0);
        public Vector3D Gryroscope = new Vector3D(0, 0, 0);
        public Vector3D MagneticField = new Vector3D(0, 0, 0);
        public Vector3D Rotation = new Vector3D(0, 0, 0);
        public Vector3D Position = new Vector3D(0, 0, 0);
        public Vector3D RawRotation = new Vector3D(0, 0, 0);

        private double m_lastUpdate = 0;
        private object UpdateLock = new object();
        private AHRS.MadgwickAHRS m_filter = new AHRS.MadgwickAHRS(0);

        private VectorFilter m_rotationFilter = new VectorFilter(MathNet.SignalProcessing.Filter.FilterType.LowPass, 0.25);

        static Vector3D QuarternionToEuler(double[] q)
        {
            double qw = q[0];
            double qx = q[1];
            double qy = q[2];
            double qz = q[3];

            var euler = new Vector3D();
            euler.X = Math.Atan2(-2 * (qy * qz - qw * qx), qw * qw - qx * qx - qy * qy + qz * qz);
            euler.Y = Math.Asin(2 * (qx * qz + qw * qy));
            euler.Z = Math.Atan2(-2 * (qx * qy - qw * qz), qw * qw + qx * qx - qy * qy - qz * qz);
            return euler;

        }


        static Matrix3D QuarternionToRotationMatrix(double[] q)
        {
            Matrix3D r = new Matrix3D();
            return r;
        }

        private Vector3D CalibrationRotation = new Vector3D(0, 0, 0);

        public void SetCalibratedPosition()
        {
            CalibrationRotation = RawRotation;
        }

        private void ReceiveDatagram(IAsyncResult result)
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
            byte[] received = m_client.EndReceive(result, ref RemoteIpEndPoint);

            String dataLine = Encoding.ASCII.GetString(received);
            String[] data = dataLine.Split(new Char[] { ',' });

            if (data.Length > 16)
            {
                double timestamp = Double.Parse(data[0]);
                if (m_lastUpdate == 0)
                {
                    m_lastUpdate = timestamp;
                }
                else
                {
                    double timestep = timestamp - m_lastUpdate;
                    m_lastUpdate = timestamp;

                    Acceleration.X = Double.Parse(data[2]);
                    Acceleration.Y = Double.Parse(data[3]);
                    Acceleration.Z = Double.Parse(data[4]);

                    Gryroscope.X = Double.Parse(data[6]);
                    Gryroscope.Y = Double.Parse(data[7]);
                    Gryroscope.Z = Double.Parse(data[8]);

                    MagneticField.X = Double.Parse(data[10]);
                    MagneticField.Y = Double.Parse(data[11]);
                    MagneticField.Z = Double.Parse(data[12]);

                    LinearAcceleration.X = Double.Parse(data[14]);
                    LinearAcceleration.Y = Double.Parse(data[15]);
                    LinearAcceleration.Z = Double.Parse(data[16]);

                    m_filter.SamplePeriod = timestep;

                    
                    m_filter.Update(Gryroscope.X, Gryroscope.Y, Gryroscope.Z, Acceleration.X, Acceleration.Y, Acceleration.Z);

                    RawRotation = (QuarternionToEuler(m_filter.Quaternion));
                    Rotation = RawRotation - CalibrationRotation;

                    Rotation = m_rotationFilter.Filter(Rotation);
                    LinearVelocity = LinearVelocity + (LinearAcceleration * timestep);
                    
                  //  Position.X = Position.X + (LinearVelocity.X * timestep);
                  //  Position.Y = Position.Y + (LinearVelocity.Y * timestep);                  
                    
                }
            }
            m_client.BeginReceive(new AsyncCallback(ReceiveDatagram), this);
        }
    }
}
