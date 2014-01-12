using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tracker
{
    /// <summary>
    /// Interaction logic for VectorVisualizer.xaml
    /// </summary>
    public partial class VectorVisualizer : UserControl
    {
        public VectorVisualizer()
        {
            InitializeComponent();
            CompositionTarget.Rendering += Update;
            

        }

        private void Update(object sender, EventArgs e)
        {
            // Create a mesh builder and add a box to it
            var meshBuilder = new MeshBuilder(false, false);

            var modelGroup = new Model3DGroup();
          
            // Create some materials
            var greenMaterial = MaterialHelper.CreateMaterial(Colors.Green);
            var redMaterial = MaterialHelper.CreateMaterial(Colors.Red);
            var blueMaterial = MaterialHelper.CreateMaterial(Colors.Blue);

            meshBuilder = new MeshBuilder(false, false);
            meshBuilder.AddArrow(new Point3D(0, 0, 0), new Point3D(3, 0, 0), 0.05);
            var mesh = meshBuilder.ToMesh(true);
            modelGroup.Children.Add(new GeometryModel3D { Geometry = mesh, Material = redMaterial});

            meshBuilder = new MeshBuilder(false, false);
            meshBuilder.AddArrow(new Point3D(0, 0, 0), new Point3D(0, 2, 0), 0.05);
            mesh = meshBuilder.ToMesh(true);
            modelGroup.Children.Add(new GeometryModel3D { Geometry = mesh, Material = blueMaterial, });

            meshBuilder = new MeshBuilder(false, false);
            meshBuilder.AddArrow(new Point3D(0, 0, 0), new Point3D(0, 0, 2), 0.05);
            mesh = meshBuilder.ToMesh(true);
            modelGroup.Children.Add(new GeometryModel3D { Geometry = mesh, Material = greenMaterial });

            Model = modelGroup;

        }

        public object Vector
        {
            get
            {
                return GetValue(VectorProperty);
            }
            set
            {
                SetValue(VectorProperty, value);
            }
        }

        public Model3D Model
        {
            get;
            set;
        }


        public static readonly DependencyProperty VectorProperty = DependencyProperty.Register("Vector", typeof(Vector3D), typeof(VectorVisualizer));
    }
}
