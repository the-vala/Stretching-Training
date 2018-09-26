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
/* -- Bibliotecas añadidas --*/
using Microsoft.Kinect;
using System.IO;
/*---------------------------*/

namespace Ejemplo_PlantillaSkeleton
{
    /// <summary>
    /// Capítulo: Reflejar el movimiento con imágenes
    /// Ejemplo: Obtener la posición de la mano derecha (De cualquier persona, no se selecciona cual)
    /// Descripción: 
    ///              Este sencillo ejemplo muestra una ventana con un círculo del cual, su movimiento, refleja el 
    ///              movimiento de la mano derecha. Conforme se mueve la mano se mueve el círculo.
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor miKinect;  //Representa el Kinect conectado

        /* ----------------------- Área para las variables ------------------------- */
        double dMano_X;            //Representa la coordenada X de la mano derecha
        double dMano_Y;            //Representa la coordenada Y de la mano derecha
        Point joint_Point = new Point(); //Permite obtener los datos del Joint
        double dXC, dYC;
        //Variables	que	almacenan	el	radio	de	cada	uno	de	los	círculos.
        double dRadioC1, dRadioC2;
        /* ------------------------------------------------------------------------- */

        public MainWindow()
        {
            InitializeComponent();

            //Calcula	la	coordenada	del	centro	del	aro
            dXC = (double)Circulo2.GetValue(Canvas.LeftProperty) + (Circulo2.Width / 2);
            dYC = (double)Circulo2.GetValue(Canvas.TopProperty) + (Circulo2.Height / 2);

            //Calcular	el	radio	de	cada	uno	de	los	círculos
            dRadioC1 = Circulo1.Width / 2;
            dRadioC2 = Circulo2.Width / 2;

            // Realizar configuraciones e iniciar el Kinect
            Kinect_Config();
        }
        /* -- Área para el método que utiliza los datos proporcionados por Kinect -- */
        /// <summary>
        /// Método que realiza las manipulaciones necesarias sobre el Skeleton trazado
        /// </summary>
        //Recibe	la	información	de	un	esqueleto	y	la	utiliza	para	hacer	que	una	elipse		
        //denominada	Puntero	siga	el	movimiento	de	la	mano	derecha
        private void usarSkeleton(Skeleton skeleton)
        {       //Extrae	la	información	del	Joint	de	la	mano	derecha
            Joint joint1 = skeleton.Joints[JointType.HandRight];
            //	Si	el	Joint	está	listo	obtener	las	coordenadas
            if (joint1.TrackingState == JointTrackingState.Tracked)
            {
                //	Obtiene	las	coordenadas	(x,	y)	del	Joint
                joint_Point = this.SkeletonPointToScreen(joint1.Position);
                dMano_X = joint_Point.X;
                dMano_Y = joint_Point.Y;
                //Emplea	las	coordenadas	del	Joint	para	mover	la	elipse	
                Puntero.SetValue(Canvas.TopProperty, dMano_Y);
                Puntero.SetValue(Canvas.LeftProperty, dMano_X);
                //	Obtiene	el	Id	de	la	persona	mapeada
                LID.Content = skeleton.TrackingId;
                //	Verificar	si	el	círculo	rojo	se	encuentra	dentro	de	la	trayectoria
                if (checarDistancia())
                {
                    Circulo1.Fill = Brushes.Yellow; //No	se	encuentra
                }
                else
                {
                    Circulo1.Fill = Brushes.Black;      //Sí	se	encuentra
                }
            }
        }
        /* ------------------------------------------------------------------------- */

        /* --------------------------- Métodos Nuevos ------------------------------ */

        /// <summary>
        /// Metodo que convierte un "SkeletonPoint" a "DepthSpace", esto nos permite poder representar las coordenadas de los Joints
        /// en nuestra ventana en las dimensiones deseadas.
        /// </summary>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convertertir un punto a "Depth Space" en una resolución de 640x480
            DepthImagePoint depthPoint = this.miKinect.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }
        /* ------------------------------------------------------------------------- */

        /// <summary>
        /// Método que realiza las configuraciones necesarias en el Kinect 
        /// así también inicia el Kinect para el envío de datos
        /// </summary>
        private void Kinect_Config()
        {
            // Buscamos el Kinect conectado con la propiedad KinectSensors, al descubrir el primero con el estado Connected
            // se asigna a la variable miKinect que lo representará (KinectSensor miKinect)
            miKinect = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);

            if (this.miKinect != null && !this.miKinect.IsRunning)
            {

                /* ------------------- Configuración del Kinect ------------------- */
                // Habilitar el SkeletonStream para permitir el trazo de "Skeleton"
                this.miKinect.SkeletonStream.Enable();

                // Enlistar al evento que se ejecuta cada vez que el Kinect tiene datos listos
                this.miKinect.SkeletonFrameReady += this.Kinect_FrameReady;
                /* ---------------------------------------------------------------- */

                // Enlistar el método que se llama cada vez que hay un cambio en el estado del Kinect
                KinectSensor.KinectSensors.StatusChanged += Kinect_StatusChanged;

                // Iniciar el Kinect
                try
                {
                    this.miKinect.Start();
                }
                catch (IOException)
                {
                    this.miKinect = null;
                }
                LEstatus.Content = "Conectado";
            }
            else
            {
                // Enlistar el método que se llama cada vez que hay un cambio en el estado del Kinect
                KinectSensor.KinectSensors.StatusChanged += Kinect_StatusChanged;
            }
        }
        /// <summary>
        /// Método que adquiere los datos que envia el Kinect, su contenido varía según la tecnología 
        /// que se esté utilizando (Cámara, SkeletonTraking, DepthSensor, etc)
        /// </summary>
        private void Kinect_FrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            // Arreglo que recibe los datos  
            Skeleton[] skeletons = new Skeleton[0];
            Skeleton skeleton;

            // Abrir el frame recibido y copiarlo al arreglo skeletons
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            // Seleccionar el primer Skeleton trazado
            skeleton = (from trackSkeleton in skeletons where trackSkeleton.TrackingState == SkeletonTrackingState.Tracked select trackSkeleton).FirstOrDefault();

            if (skeleton == null)
            {
                LID.Content = "0";
                return;
            }
            LID.Content = skeleton.TrackingId;

            // Enviar el Skelton a usar
            this.usarSkeleton(skeleton);
        }

        private bool checarDistancia()
        {
            //Obtiene	la	coordenada	del	centro	del	círculo	que	mueve	la	persona
            double dX1 = (double)Puntero.GetValue(Canvas.LeftProperty) + (Puntero.Width / 2);
            double dY1 = (double)Puntero.GetValue(Canvas.TopProperty) + (Puntero.Height / 2);
            //Calcula	la	distancia	entre	el	centro	del	Puntero	(círculo	rojo)	y
            //el	centro	del	aro
            double dDistancia = Math.Sqrt(Math.Pow(dXC - dX1, 2) + Math.Pow(dYC - dY1, 2));
            //Compara	la	distancia	calculada	con	los	radios	de	los	dos	círculos	que	forman
            //el	aro	en	el	entendido	de	que	si	la	distancia	es	mayor	al	círculo	más	grande
            //o	menor	al	círculo	más	pequeño,	entonces	el	círculo	rojo	
            //se	ha	salido	del	trayecto.
            return (dDistancia > dRadioC1 || dDistancia < dRadioC2);
        }

        /// <summary>
        /// Método que configura del Kinect de acuerdo a su estado(conectado, desconectado, etc),
        /// su contenido varia según la tecnología que se esté utilizando (Cámara, SkeletonTraking, DepthSensor, etc)
        /// </summary>
        private void Kinect_StatusChanged(object sender, StatusChangedEventArgs e)
        {

            switch (e.Status)
            {
                case KinectStatus.Connected:
                    if (this.miKinect == null)
                    {
                        this.miKinect = e.Sensor;
                    }

                    if (this.miKinect != null && !this.miKinect.IsRunning)
                    {
                        /* ------------------- Configuración del Kinect ------------------- */
                        // Habilitar el SkeletonStream para permitir el trazo de "Skeleton"
                        this.miKinect.SkeletonStream.Enable();

                        // Enlistar al evento que se ejecuta cada vez que el Kinect tiene datos listos
                        this.miKinect.SkeletonFrameReady += this.Kinect_FrameReady;
                        /* ---------------------------------------------------------------- */

                        // Iniciar el Kinect
                        try
                        {
                            this.miKinect.Start();
                        }
                        catch (IOException)
                        {
                            this.miKinect = null;
                        }
                        LEstatus.Content = "Conectado";
                    }
                    break;
                case KinectStatus.Disconnected:
                    if (this.miKinect == e.Sensor)
                    {
                        /* ------------------- Configuración del Kinect ------------------- */
                        this.miKinect.SkeletonFrameReady -= this.Kinect_FrameReady;
                        /* ---------------------------------------------------------------- */

                        this.miKinect.Stop();
                        this.miKinect = null;
                        LEstatus.Content = "Desconectado";

                    }
                    break;
            }
        }
        /// <summary>
        /// Método que libera los recursos del Kinect cuando se termina la aplicación
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.miKinect != null && this.miKinect.IsRunning)
            {
                /* ------------------- Configuración del Kinect ------------------- */
                this.miKinect.SkeletonFrameReady -= this.Kinect_FrameReady;
                /* ---------------------------------------------------------------- */

                this.miKinect.Stop();
            }
        }
    }
}
