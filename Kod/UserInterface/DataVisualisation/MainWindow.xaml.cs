using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;

namespace DataVisualisation
{
    public enum VisualisationMode { RotationMode, TranslationMode, FullPositionMode };

    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MainWindow : UserControl
    {
        private ModelVisual3D mCube;
        private VisualisationMode mode;
        private Point3D p1, p2, p3, p4, p5, p6, p7, p8;
        private float accX, accY, accZ, angX, angY, angZ;
        
        public MainWindow()
        {
            InitializeComponent();
            BuildScene();
        }

        public VisualisationMode WindowMode
        {
            get { return mode; }
            set { mode = value; }
        }

        public float AccelerationX
        {
            get { return accX; }
            set { accX = value; }
        }

        public float AccelerationY
        {
            get { return accY; }
            set { accY = value; }
        }

        public float AccelerationZ
        {
            get { return accZ; }
            set { accZ = value; }
        }

        public float AngleX
        {
            get { return angX; }
            set { angX = value; }
        }

        public float AngleY
        {
            get { return angY; }
            set { angY = value; }
        }

        public float AngleZ
        {
            get { return angZ; }
            set { angZ = value; }
        }

        public void ApplyTransformation()
        {
            Transform3DGroup group = mCube.Transform as Transform3DGroup;

            AxisAngleRotation3D aX = new AxisAngleRotation3D();
            aX.Axis = new Vector3D(1, 0, 0);
            aX.Angle = angX;
            RotateTransform3D r1 = new RotateTransform3D(aX);

            AxisAngleRotation3D aY = new AxisAngleRotation3D();
            aY.Axis = new Vector3D(0, 1, 0);
            aY.Angle = angY;
            RotateTransform3D r2 = new RotateTransform3D(aY);

            AxisAngleRotation3D aZ= new AxisAngleRotation3D();
            aZ.Axis = new Vector3D(0, 0, 1);
            aZ.Angle = angZ;
            RotateTransform3D r3 = new RotateTransform3D(aZ);

            TranslateTransform3D t1 = new TranslateTransform3D(accX, 0, 0);
            TranslateTransform3D t2 = new TranslateTransform3D(0, accZ, 0);
            TranslateTransform3D t3 = new TranslateTransform3D(0, 0, accY);

            if (mode != VisualisationMode.TranslationMode)
            {
                group.Children.Add(r1);
                group.Children.Add(r2);
                group.Children.Add(r3);
            }

            if (mode != VisualisationMode.RotationMode)
            {
                group.Children.Add(t1);
                group.Children.Add(t2);
                group.Children.Add(t3);
            }
        }
        
        private Model3DGroup MeshTriangle(Point3D p0, Point3D p1, Point3D p2, Color color)
        {
            Model3DGroup modelGroup;
            MeshGeometry3D meshGeometry;

            meshGeometry = new MeshGeometry3D();

            meshGeometry.Positions.Add(p0);
            meshGeometry.Positions.Add(p1);
            meshGeometry.Positions.Add(p2);

            meshGeometry.TriangleIndices.Add(0);
            meshGeometry.TriangleIndices.Add(1);
            meshGeometry.TriangleIndices.Add(2);

            modelGroup = new Model3DGroup();
            modelGroup.Children.Add(new GeometryModel3D(meshGeometry, new DiffuseMaterial(new SolidColorBrush(color))));

            return modelGroup;
        }

        public void BuildScene()
        {
            p1 = new Point3D(-4, -4, -4);
            p2 = new Point3D(4, -4, -4);
            p3 = new Point3D(4, -4, 4);
            p4 = new Point3D(-4, -4, 4);
            p5 = new Point3D(-4, 4, -4);
            p6 = new Point3D(4, 4, -4);
            p7 = new Point3D(4, 4, 4);
            p8 = new Point3D(-4, 4, 4);
            this.ClearView();

            mCube = new ModelVisual3D();
            
            Model3DGroup cubeGroup = new Model3DGroup();

            //front side
            Color red = Colors.OrangeRed;
            cubeGroup.Children.Add(MeshTriangle(p4, p3, p7, red));
            cubeGroup.Children.Add(MeshTriangle(p4, p7, p8, red));

            //right side 
            Color blue = Colors.CornflowerBlue;
            cubeGroup.Children.Add(MeshTriangle(p3, p2, p6, blue));
            cubeGroup.Children.Add(MeshTriangle(p3, p6, p7, blue));

            //back side
            Color darkRed = Colors.DarkRed;
            cubeGroup.Children.Add(MeshTriangle(p2, p1, p5, darkRed));
            cubeGroup.Children.Add(MeshTriangle(p2, p5, p6, darkRed));

            //left side
            Color darkBLue = Colors.DarkSlateBlue;
            cubeGroup.Children.Add(MeshTriangle(p1, p4, p8, darkBLue));
            cubeGroup.Children.Add(MeshTriangle(p1, p8, p5, darkBLue));

            //top side
            Color green = Colors.LawnGreen;
            cubeGroup.Children.Add(MeshTriangle(p8, p7, p6, green));
            cubeGroup.Children.Add(MeshTriangle(p8, p6, p5, green));

            //bottom side
            Color darkGreen = Colors.DarkSeaGreen;
            cubeGroup.Children.Add(MeshTriangle(p3, p4, p1, darkGreen));
            cubeGroup.Children.Add(MeshTriangle(p3, p1, p2, darkGreen));

            mCube.Content = cubeGroup;
            mCube.Transform = new Transform3DGroup();

            //axis
            _3DTools.ScreenSpaceLines3D xLine = new _3DTools.ScreenSpaceLines3D();
            xLine.Points.Add(new Point3D(-8, 0, 0));
            xLine.Points.Add(new Point3D(8, 0, 0));
            xLine.Thickness = 2;
            xLine.Color = Colors.Blue;

            _3DTools.ScreenSpaceLines3D yLine = new _3DTools.ScreenSpaceLines3D();
            yLine.Points.Add(new Point3D(0, -8, 0));
            yLine.Points.Add(new Point3D(0, 8, 0));
            yLine.Thickness = 2;
            yLine.Color = Colors.Red;

            _3DTools.ScreenSpaceLines3D zLine = new _3DTools.ScreenSpaceLines3D();
            zLine.Points.Add(new Point3D(0, 0, -8));
            zLine.Points.Add(new Point3D(0, 0, 8));
            zLine.Thickness = 2;
            zLine.Color = Colors.Yellow;

            mainView.Children.Add(mCube);
            mainView.Children.Add(xLine);
            mainView.Children.Add(yLine);
            mainView.Children.Add(zLine);
        }

        private void ClearView()
        {
            ModelVisual3D viewPortChildren;
            for (int i = mainView.Children.Count - 1; i >= 0; i--)
            {
                viewPortChildren = (ModelVisual3D)mainView.Children[i];
                if (viewPortChildren.Content is DirectionalLight == false)
                {
                    mainView.Children.RemoveAt(i);
                }
            }
        }
    }
}
