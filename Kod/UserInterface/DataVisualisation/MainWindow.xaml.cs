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
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MainWindow : UserControl
    {
        private ModelVisual3D mCube;
        private Point3D p1 = new Point3D(-4, 4, -4);
        private Point3D p2 = new Point3D(-4, 4, 4);
        private Point3D p3 = new Point3D(4, 4, 4);
        private Point3D p4 = new Point3D(4, 4, -4);
        private Point3D p5 = new Point3D(-4, -4, -4);
        private Point3D p6 = new Point3D(-4, -4, 4);
        private Point3D p7 = new Point3D(4, -4, 4);
        private Point3D p8 = new Point3D(4, -4, -4);

        public MainWindow()
        {
            InitializeComponent();
            BuildScene();
        }

        private Model3DGroup meshTriangle(Point3D p0, Point3D p1, Point3D p2)
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
            modelGroup.Children.Add(new GeometryModel3D(meshGeometry, new DiffuseMaterial(new SolidColorBrush(Colors.LawnGreen))));

            Vector3D normal = CalculateNormalVector(p0, p1, p2);
            meshGeometry.Normals.Add(normal);
            meshGeometry.Normals.Add(normal);
            meshGeometry.Normals.Add(normal);

            return modelGroup;
        }

        private Vector3D CalculateNormalVector(Point3D p1, Point3D p2, Point3D p3)
        {
            Vector3D vec1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            Vector3D vec2 = new Vector3D(p3.X - p2.X, p3.Y - p2.Y, p3.Z - p2.Z);

            return Vector3D.CrossProduct(vec1, vec2);
        }

        private void BuildScene()
        {
            mCube = new ModelVisual3D();

            Model3DGroup cubeGroup = new Model3DGroup();

            //top side
            cubeGroup.Children.Add(meshTriangle(p1, p2, p3));
            cubeGroup.Children.Add(meshTriangle(p1, p3, p4));

            //bottom side 
            cubeGroup.Children.Add(meshTriangle(p5, p6, p7));
            cubeGroup.Children.Add(meshTriangle(p5, p7, p8));

            //back side
            cubeGroup.Children.Add(meshTriangle(p1, p8, p4));
            cubeGroup.Children.Add(meshTriangle(p1, p5, p8));

            //front side
            cubeGroup.Children.Add(meshTriangle(p2, p7, p3));
            cubeGroup.Children.Add(meshTriangle(p2, p6, p7));

            //left side
            cubeGroup.Children.Add(meshTriangle(p5, p1, p2));
            cubeGroup.Children.Add(meshTriangle(p5, p2, p6));

            //right side
            cubeGroup.Children.Add(meshTriangle(p8, p4, p3));
            cubeGroup.Children.Add(meshTriangle(p8, p3, p7));

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

        

        public void ApplyAcceleration(float accelerationX, float accelerationY, float accelerationZ)
        {
            p1.X = -4 + accelerationX;  p1.Y = 4 + accelerationY;   p1.Z = -4 + accelerationZ;
            p2.X = -4 + accelerationX;  p2.Y = 4 + accelerationY;   p2.Z = 4 + accelerationZ;
            p3.X = 4 + accelerationX;   p3.Y = 4 + accelerationY;   p3.Z = 4 + accelerationZ;
            p4.X = 4 + accelerationX;   p4.Y = 4 + accelerationY;   p4.Z = -4 + accelerationZ;
            p5.X = -4 + accelerationX;  p5.Y = -4 + accelerationY;  p5.Z = -4 + accelerationZ;
            p6.X = -4 + accelerationX;  p6.Y = -4 + accelerationY;  p6.Z = 4 + accelerationZ;
            p7.X = 4 + accelerationX;   p7.Y = -4 + accelerationY;  p7.Z = 4 + accelerationZ;
            p8.X = 4 + accelerationX;   p8.Y = -4 + accelerationY;  p8.Z = -4 + accelerationZ;
        }

        public void ApplyRotation(float angleX, float angleY, float angleZ)
        {
            p1.X = -4 + Math.Cos(angleY) + Math.Sin(angleZ);    p1.Y = 4 + Math.Cos(angleX) + Math.Sin(angleZ);     p1.Z = -4 + Math.Cos(angleX) + Math.Sin(angleY);
            p2.X = -4 + Math.Cos(angleY) + Math.Sin(angleZ);    p2.Y = 4 + Math.Cos(angleX) + Math.Sin(angleZ);     p2.Z = 4 + Math.Cos(angleX) + Math.Sin(angleY);
            p3.X = 4 + Math.Cos(angleY) + Math.Sin(angleZ);     p3.Y = 4 + Math.Cos(angleX) + Math.Sin(angleZ);     p3.Z = 4 + Math.Cos(angleX) + Math.Sin(angleY);
            p4.X = 4 + Math.Cos(angleY) + Math.Sin(angleZ);     p4.Y = 4 + Math.Cos(angleX) + Math.Sin(angleZ);     p4.Z = -4 + Math.Cos(angleX) + Math.Sin(angleY);
            p5.X = -4 + Math.Cos(angleY) + Math.Sin(angleZ);    p5.Y = -4 + Math.Cos(angleX) + Math.Sin(angleZ);    p5.Z = -4 + Math.Cos(angleX) + Math.Sin(angleY);
            p6.X = -4 + Math.Cos(angleY) + Math.Sin(angleZ);    p6.Y = -4 + Math.Cos(angleX) + Math.Sin(angleZ);    p6.Z = 4 + Math.Cos(angleX) + Math.Sin(angleY);
            p7.X = 4 + Math.Cos(angleY) + Math.Sin(angleZ);     p7.Y = -4 + Math.Cos(angleX) + Math.Sin(angleZ);    p7.Z = 4 + Math.Cos(angleX) + Math.Sin(angleY);
            p8.X = 4 + Math.Cos(angleY) + Math.Sin(angleZ);     p8.Y = -4 + Math.Cos(angleX) + Math.Sin(angleZ);    p8.Z = -4 + Math.Cos(angleX) + Math.Sin(angleY);
        }
    }
}
