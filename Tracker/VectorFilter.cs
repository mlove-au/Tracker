using MathNet.SignalProcessing.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using MathNet.SignalProcessing.Filter.Median;
namespace Tracker
{
    public class VectorFilter
    {
        private IOnlineFilter m_filterX;
        private IOnlineFilter m_filterY;
        private IOnlineFilter m_filterZ;

        public VectorFilter(FilterType type, double cutoff)
        {

            
            switch (type)
            {
                case FilterType.LowPass:                    
                    m_filterX = OnlineFilter.CreateLowpass(ImpulseResponse.Finite, 1, cutoff);
                    m_filterY = OnlineFilter.CreateLowpass(ImpulseResponse.Finite, 1, cutoff);
                    m_filterZ = OnlineFilter.CreateLowpass(ImpulseResponse.Finite, 1, cutoff); 

            /*        m_filterX = new OnlineMedianFilter(100);
                    m_filterY = new OnlineMedianFilter(100);
                    m_filterZ = new OnlineMedianFilter(100);*/

                    break;

                case FilterType.HighPass:
                    m_filterX = OnlineFilter.CreateHighpass(ImpulseResponse.Finite, 1, cutoff);
                    m_filterY = OnlineFilter.CreateHighpass(ImpulseResponse.Finite, 1, cutoff);
                    m_filterZ = OnlineFilter.CreateHighpass(ImpulseResponse.Finite, 1, cutoff);                    
                    break;
            }
        }

        public Vector3D Filter(Vector3D v)
        {
            return new Vector3D(m_filterX.ProcessSample(v.X), m_filterY.ProcessSample(v.Y), m_filterZ.ProcessSample(v.Z));
        }
    }
}
