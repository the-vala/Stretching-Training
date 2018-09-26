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
using System.Windows.Threading;
/*---------------------------*/

namespace Ejemplo_PlantillaSkeleton
{
    public partial class MainWindow : Window
    {
        private KinectSensor miKinect;  //Representa el Kinect conectado

        /* ----------------------- Área para las variables ------------------------- */
        double joint_X;            //Representa la coordenada X del joint
        double joint_Y;            //Representa la coordenada Y del joint
        Point joint_Point = new Point(); //Permite obtener los datos del Joint
        double dXC, dYC;
        //Variables	que	almacenan	el	radio	de	cada	uno	de	los	círculos.
        double dRadioC1, dRadioC2;
        //Timer animación circulos
        DispatcherTimer goalHTimer;
        //Timer barra de progreso
        DispatcherTimer progressTimer;
        int iCont = 8;
        int Ejercicio = 1;
        /* ------------------------------------------------------------------------- */

        public MainWindow()
        {
            InitializeComponent();
            //Calcular	el	radio	de	cada	uno	de	los	círculos
            dRadioC1 = CirculoOutRH.Width / 2;
            dRadioC2 = CirculoInRH.Width / 2;

            goalHTimer = new DispatcherTimer();
            goalHTimer.Interval = new TimeSpan(0, 0, 0, 0, 40);
            goalHTimer.Tick += new EventHandler(MoveHandGoal);
            goalHTimer.IsEnabled = true;

            //Timer para la barra de progreso
            progressTimer = new DispatcherTimer();
            progressTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            progressTimer.Tick += new EventHandler(TimerBar);
            progressTimer.IsEnabled = false;

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
            Joint rHand = skeleton.Joints[JointType.HandRight];
            Joint lHand = skeleton.Joints[JointType.HandLeft];
            Joint rElbow = skeleton.Joints[JointType.ElbowRight];
            Joint lElbow = skeleton.Joints[JointType.ElbowLeft];
            Joint rShoulder = skeleton.Joints[JointType.ShoulderRight];
            Joint lShoulder = skeleton.Joints[JointType.ShoulderLeft];
            Joint cShoulder = skeleton.Joints[JointType.ShoulderCenter];
            Joint head = skeleton.Joints[JointType.Head];
            Joint spine = skeleton.Joints[JointType.Spine];
            //	Si	head está	listo	obtener	las	coordenadas
            if (head.TrackingState == JointTrackingState.Tracked)
            {
                //	Obtiene	las	coordenadas	(x,	y)	del	Joint
                joint_Point = this.SkeletonPointToScreen(head.Position);
                joint_X = joint_Point.X;
                joint_Y = joint_Point.Y;
                //Emplea	las	coordenadas	del	Joint	para	mover	la	elipse	
                pointerHead.SetValue(Canvas.TopProperty, joint_Y);
                pointerHead.SetValue(Canvas.LeftProperty, joint_X);
                //	Obtiene	el	Id	de	la	persona	mapeada
                LID.Content = skeleton.TrackingId;
            }
            //	Si	rHand está	listo	obtener	las	coordenadas
            if (rHand.TrackingState == JointTrackingState.Tracked)
            {
                //	Obtiene	las	coordenadas	(x,	y)	del	Joint
                joint_Point = this.SkeletonPointToScreen(rHand.Position);
                joint_X = joint_Point.X;
                joint_Y = joint_Point.Y;
                //Emplea	las	coordenadas	del	Joint	para	mover	la	elipse	
                pointerRHand.SetValue(Canvas.TopProperty, joint_Y);
                pointerRHand.SetValue(Canvas.LeftProperty, joint_X);
                //	Obtiene	el	Id	de	la	persona	mapeada
                LID.Content = skeleton.TrackingId;
                if (checarDistancia(pointerRHand, CirculoInRH))
                {
                    CirculoOutRH.Fill = Brushes.Red; //No	se	encuentra
                }
                else
                {
                    CirculoOutRH.Fill = Brushes.Green;      //Sí	se	encuentra
                }
            }
            //	Si	lHand está	listo	obtener	las	coordenadas
            if (lHand.TrackingState == JointTrackingState.Tracked)
            {
                //	Obtiene	las	coordenadas	(x,	y)	del	Joint
                joint_Point = this.SkeletonPointToScreen(lHand.Position);
                joint_X = joint_Point.X;
                joint_Y = joint_Point.Y;
                //Emplea	las	coordenadas	del	Joint	para	mover	la	elipse	
                pointerLHand.SetValue(Canvas.TopProperty, joint_Y);
                pointerLHand.SetValue(Canvas.LeftProperty, joint_X);
                //	Obtiene	el	Id	de	la	persona	mapeada
                LID.Content = skeleton.TrackingId;
                if (checarDistancia(pointerLHand, CirculoInLH))
                {
                    CirculoOutLH.Fill = Brushes.Red; //No	se	encuentra
                }
                else
                {
                    CirculoOutLH.Fill = Brushes.Green;      //Sí	se	encuentra
                }
            }
            //	Si	rShoulder está	listo	obtener	las	coordenadas
            if (rShoulder.TrackingState == JointTrackingState.Tracked)
            {
                //	Obtiene	las	coordenadas	(x,	y)	del	Joint
                joint_Point = this.SkeletonPointToScreen(rShoulder.Position);
                joint_X = joint_Point.X;
                joint_Y = joint_Point.Y;
                //Emplea	las	coordenadas	del	Joint	para	mover	la	elipse	
                pointerRShoulder.SetValue(Canvas.TopProperty, joint_Y);
                pointerRShoulder.SetValue(Canvas.LeftProperty, joint_X);
                //	Obtiene	el	Id	de	la	persona	mapeada
                LID.Content = skeleton.TrackingId;
            }
            //	Si	lShoulder está	listo	obtener	las	coordenadas
            if (lShoulder.TrackingState == JointTrackingState.Tracked)
            {
                //	Obtiene	las	coordenadas	(x,	y)	del	Joint
                joint_Point = this.SkeletonPointToScreen(lShoulder.Position);
                joint_X = joint_Point.X;
                joint_Y = joint_Point.Y;
                //Emplea	las	coordenadas	del	Joint	para	mover	la	elipse	
                pointerLShoulder.SetValue(Canvas.TopProperty, joint_Y);
                pointerLShoulder.SetValue(Canvas.LeftProperty, joint_X);
                //	Obtiene	el	Id	de	la	persona	mapeada
                LID.Content = skeleton.TrackingId;
            }
            //	Si	cShoulder está	listo	obtener	las	coordenadas
            if (cShoulder.TrackingState == JointTrackingState.Tracked)
            {
                //	Obtiene	las	coordenadas	(x,	y)	del	Joint
                joint_Point = this.SkeletonPointToScreen(cShoulder.Position);
                joint_X = joint_Point.X;
                joint_Y = joint_Point.Y;
                //Emplea	las	coordenadas	del	Joint	para	mover	la	elipse	
                pointerCShoulder.SetValue(Canvas.TopProperty, joint_Y);
                pointerCShoulder.SetValue(Canvas.LeftProperty, joint_X);
                //	Obtiene	el	Id	de	la	persona	mapeada
                LID.Content = skeleton.TrackingId;
            }
            //	Si	rElbow está	listo	obtener	las	coordenadas
            if (rElbow.TrackingState == JointTrackingState.Tracked)
            {
                //	Obtiene	las	coordenadas	(x,	y)	del	Joint
                joint_Point = this.SkeletonPointToScreen(rElbow.Position);
                joint_X = joint_Point.X;
                joint_Y = joint_Point.Y;
                //Emplea	las	coordenadas	del	Joint	para	mover	la	elipse	
                pointerRElbow.SetValue(Canvas.TopProperty, joint_Y);
                pointerRElbow.SetValue(Canvas.LeftProperty, joint_X);
                //	Obtiene	el	Id	de	la	persona	mapeada
                LID.Content = skeleton.TrackingId;
            }
            //	Si	lElbow está	listo	obtener	las	coordenadas
            if (lElbow.TrackingState == JointTrackingState.Tracked)
            {
                //	Obtiene	las	coordenadas	(x,	y)	del	Joint
                joint_Point = this.SkeletonPointToScreen(lElbow.Position);
                joint_X = joint_Point.X;
                joint_Y = joint_Point.Y;
                //Emplea	las	coordenadas	del	Joint	para	mover	la	elipse	
                pointerLElbow.SetValue(Canvas.TopProperty, joint_Y);
                pointerLElbow.SetValue(Canvas.LeftProperty, joint_X);
                //	Obtiene	el	Id	de	la	persona	mapeada
                LID.Content = skeleton.TrackingId;
            }
            //	Si	spine está	listo	obtener	las	coordenadas
            if (spine.TrackingState == JointTrackingState.Tracked)
            {
                //	Obtiene	las	coordenadas	(x,	y)	del	Joint
                joint_Point = this.SkeletonPointToScreen(spine.Position);
                joint_X = joint_Point.X;
                joint_Y = joint_Point.Y;
                //Emplea	las	coordenadas	del	Joint	para	mover	la	elipse	
                pointerSpine.SetValue(Canvas.TopProperty, joint_Y);
                pointerSpine.SetValue(Canvas.LeftProperty, joint_X);
                //	Obtiene	el	Id	de	la	persona	mapeada
                LID.Content = skeleton.TrackingId;
            }
            if (checarDistancia(pointerRHand, CirculoInRH) && checarDistancia(pointerLHand, CirculoInLH))
                progressTimer.IsEnabled = false;
            else
                progressTimer.IsEnabled = true;
        }
        /* ------------------------------------------------------------------------- */

        /* --------------------------- Métodos Nuevos ------------------------------ */
        
        private void TimerBar(object sender, EventArgs e)
        {
            if (iCont == 0)
            {
                Ejercicio++;
                ejercicio.Content = "Ejercicio# " + Ejercicio;
                iCont = 8;
                return;
            }
            progressbar.Maximum = 8;
            progressbar.Value++;
            iCont--;
            tiempo.Content = "Tiempo: " + iCont;

        }
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
        
        double anguloRH = 360;
        double anguloLH = 0;
        private void MoveHandGoal(object sender, EventArgs e)
        {
            switch(Ejercicio)
            {
                case 1:
                    anguloRH -= 10;
                    GoalRH.RenderTransform = new RotateTransform(anguloRH);
                    anguloLH += 10;
                    GoalLH.RenderTransform = new RotateTransform(anguloLH);
                    break;
                case 2:
                    CirculoInRH.Visibility = Visibility.Hidden;
                    CirculoInLH.Visibility = Visibility.Hidden;
                    CirculoOutRH.Visibility = Visibility.Hidden;
                    CirculoOutLH.Visibility = Visibility.Hidden;
                    break;
            }
            
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

        private bool checarDistancia(Ellipse Puntero, Ellipse Circulo)
        {
            dXC = (double)Circulo.GetValue(Canvas.LeftProperty) + (Circulo.Width / 2);
            dYC = (double)Circulo.GetValue(Canvas.TopProperty) + (Circulo.Height / 2);
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
