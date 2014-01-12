using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Threading;
using System.Windows.Input;

namespace Tracker
{
    public class MainWindowModel : PropertyChangedBase
    {
        private IMUReader m_reader;

        public MainWindowModel()
        {
            var modelGroup = new Model3DGroup();

            // Create a mesh builder and add a box to it
            var meshBuilder = new MeshBuilder(false, false);

            meshBuilder.AddBox(new Rect3D(-1.0, -0.5, -0.25, 2, 1, 0.5));

            // Create a mesh from the builder (and freeze it)
            var mesh = meshBuilder.ToMesh(true);

            // Create some materials
            var greenMaterial = MaterialHelper.CreateMaterial(Colors.Green);
            var redMaterial = MaterialHelper.CreateMaterial(Colors.Red);
            var blueMaterial = MaterialHelper.CreateMaterial(Colors.Blue);
            var insideMaterial = MaterialHelper.CreateMaterial(Colors.Yellow);
            var yellowMaterial = MaterialHelper.CreateMaterial(Colors.Yellow);


            modelGroup.Children.Add(new GeometryModel3D { Geometry = mesh, Material = greenMaterial, BackMaterial = insideMaterial });

            meshBuilder = new MeshBuilder(false, false);
            meshBuilder.AddArrow(new Point3D(0, 0, 0), new Point3D(3, 0, 0), 0.05);
            mesh = meshBuilder.ToMesh(true);
            modelGroup.Children.Add(new GeometryModel3D { Geometry = mesh, Material = redMaterial, BackMaterial = insideMaterial });

            meshBuilder = new MeshBuilder(false, false);
            meshBuilder.AddArrow(new Point3D(0, 0, 0), new Point3D(0, 2, 0), 0.05);
            mesh = meshBuilder.ToMesh(true);
            modelGroup.Children.Add(new GeometryModel3D { Geometry = mesh, Material = blueMaterial, BackMaterial = insideMaterial });

            meshBuilder = new MeshBuilder(false, false);
            meshBuilder.AddArrow(new Point3D(0, 0, 0), new Point3D(0, 0, 2), 0.05);
            mesh = meshBuilder.ToMesh(true);
            modelGroup.Children.Add(new GeometryModel3D { Geometry = mesh, Material = yellowMaterial, BackMaterial = insideMaterial });

            Model = modelGroup;

            CompositionTarget.Rendering += UpdateModel;
            m_reader = new IMUReader();


        }


        private void AddAxisRotation(Transform3DGroup transform, Vector3D axis, double rotation)
        {
            RotateTransform3D myRotateTransform3D = new RotateTransform3D();
            AxisAngleRotation3D myAxisAngleRotation3d = new AxisAngleRotation3D();
            myAxisAngleRotation3d.Axis = axis;
            myAxisAngleRotation3d.Angle = rotation;
            myRotateTransform3D.Rotation = myAxisAngleRotation3d;

            transform.Children.Add(myRotateTransform3D);
        }


        private void UpdateModel(object source, EventArgs e)
        {

            // Add the rotation transform to a Transform3DGroup
            Transform3DGroup myTransform3DGroup = new Transform3DGroup();
            AddAxisRotation(myTransform3DGroup, new Vector3D(1, 0, 0), m_reader.Rotation.X * (180.0 / Math.PI));
            AddAxisRotation(myTransform3DGroup, new Vector3D(0, 1, 0), m_reader.Rotation.Y * (180.0 / Math.PI));
            AddAxisRotation(myTransform3DGroup, new Vector3D(0, 0, 1), m_reader.Rotation.Z * (180.0 / Math.PI));
            var t = new TranslateTransform3D();
            t.OffsetX = m_reader.Position.X;
            t.OffsetY = m_reader.Position.Y;
            t.OffsetZ = m_reader.Position.Z;

            myTransform3DGroup.Children.Add(t);

            this.Rotation = m_reader.Rotation * 180.0 / Math.PI;
            this.Model.Transform = myTransform3DGroup;
            NotifyOfPropertyChange(() => Model);

        }


        public void Calibrate()
        {
            m_reader.SetCalibratedPosition();
        }



        private Vector3D _rotation;
        public Vector3D Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                NotifyOfPropertyChange(() => Rotation);
            }
        }

        public double RotationFilterCutoff
        {
            get { return m_reader.RotationFilterCutoff; }
            set
            {
                m_reader.RotationFilterCutoff = value;
                NotifyOfPropertyChange(() => RotationFilterCutoff);
            }
        }


        private Model3D _model;
        public Model3D Model
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;
                NotifyOfPropertyChange(() => Model);
            }
        }
    }
}
