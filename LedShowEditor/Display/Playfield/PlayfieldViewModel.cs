﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using LedShowEditor.ViewModels;
using MahApps.Metro.Controls;
using Path = System.Windows.Shapes.Path;

namespace LedShowEditor.Display.Playfield
{
    [Export(typeof (IPlayfield))]
    internal class PlayfieldViewModel : Screen, IPlayfield
    {

        #region Properties

        public string PlayfieldImageName
        {
            get { return _playfieldImageName; }
            set
            {
                _playfieldImageName = value;
                NotifyOfPropertyChange(() => PlayfieldImageName);
            }
        }

        public ImageSource PlayfieldImage
        {
            get { return _playfieldImage; }
            set
            {
                _playfieldImage = value;
                NotifyOfPropertyChange(() => PlayfieldImage);
            }
        }

        public bool IsPlayfieldVisible
        {
            get { return _isPlayfieldVisible; }
            set
            {
                _isPlayfieldVisible = value;
                NotifyOfPropertyChange(() => IsPlayfieldVisible);
            }
        }


        public double PlayfieldWidth
        {
            get { return _playfieldWidth; }
            set
            {
                _playfieldWidth = value;
                NotifyOfPropertyChange(() => PlayfieldWidth);

                // Update ScaleFactor
                ScaleFactorX = PlayfieldWidth/100;
            }
        }

        public double PlayfieldHeight
        {
            get { return _playfieldHeight; }
            set
            {
                _playfieldHeight = value;
                NotifyOfPropertyChange(() => PlayfieldHeight);

                // Update ScaleFactor
                ScaleFactorY = PlayfieldHeight/100;
            }
        }

        public double ScaleFactorX
        {
            get { return _scaleFactorX; }
            set
            {
                _scaleFactorX = value;
                NotifyOfPropertyChange(() => ScaleFactorX);
            }
        }

        public double ScaleFactorY
        {
            get { return _scaleFactorY; }
            set
            {
                _scaleFactorY = value;
                NotifyOfPropertyChange(() => ScaleFactorY);
            }
        }

        public double PlayfieldToLedsScale
        {
            get { return _playfieldToLedsScale; }
            set
            {
                _playfieldToLedsScale = value;
                NotifyOfPropertyChange(() => PlayfieldToLedsScale);
            }
        }


        public ILeds LedsVm { get; set; }

        public LedViewModel SelectedLed { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        [ImportingConstructor]
        public PlayfieldViewModel(IEventAggregator eventAggregator, ILeds ledsViewModel)
        {
            _eventAggregator = eventAggregator;
            LedsVm = ledsViewModel;

            PlayfieldWidth = 400;
            PlayfieldHeight = 800;

            _playfieldToLedsScale = 0.25;
            _isPlayfieldVisible = true;
        }

        #endregion

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _eventAggregator.Subscribe(this);
        }

        public void ClearImage()
        {
            _playfieldImagePath = "";
            PlayfieldImage = null;
        }

        public void UpdateImageLocation(string imageLocation)
        {
            _playfieldImageLocation = imageLocation;
        }

        public void UpdateImage(string imageFilename)
        {
            if (!string.IsNullOrEmpty(imageFilename) && File.Exists(imageFilename))
            {
                if (Directory.Exists(_playfieldImageLocation))
                {
                    PlayfieldImageName = System.IO.Path.GetFileName(imageFilename);
                    if (!string.Equals(imageFilename, _playfieldImageLocation + PlayfieldImageName))
                    {
                        File.Copy(imageFilename, _playfieldImageLocation + PlayfieldImageName, true);
                    }
                    PlayfieldImage = new BitmapImage(new Uri(_playfieldImageLocation + PlayfieldImageName));
                }
            }
        }

  

        #region Handle Mouse movement / Dragging of Device

        public void MouseEnter(object source)
        {
            var ledGeom = source as Path;
            if (ledGeom != null)
            {
                var activeLed = ledGeom.DataContext as LedViewModel;
                if (activeLed != null)
                {
                    activeLed.IsMouseOver = true;
                }
            }
        }

        public void MouseLeave(object source)
        {
            var ledGeom = source as Path;
            if (ledGeom != null)
            {
                var activeLed = ledGeom.DataContext as LedViewModel;
                if (activeLed != null)
                {
                    activeLed.IsMouseOver = false;
                }
            }
        }

        public Point StartingPoint { get; set; }

        public void MouseDown(object source)
        {
            var ledGeom = source as Path;
            if (ledGeom != null)
            {
                StartingPoint = Mouse.GetPosition(Application.Current.MainWindow);

                // Find the Led we are moving and set it to selected device
                var activeLed = ledGeom.DataContext as LedViewModel;
                if (activeLed != null)
                {
                    SelectedLed = activeLed;
                    SelectedLed.IsSelected = true;
                }
            }
        }

        public void MouseUp(object source)
        {
            if (SelectedLed != null && !SelectedLed.IsMouseOver)
            {
                // De-Select selected Led
                SelectedLed.IsSelected = false;
                SelectedLed = null;
            }
        }

        public void MouseMove(object source)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed && SelectedLed != null)
            {
                var currentPoint = Mouse.GetPosition(Application.Current.MainWindow);
                var xDelta = currentPoint.X - StartingPoint.X;
                var yDelta = currentPoint.Y - StartingPoint.Y;

                SelectedLed.LocationX = SelectedLed.LocationX + (double) (xDelta/ScaleFactorX);
                SelectedLed.LocationY = SelectedLed.LocationY + (double) (yDelta/ScaleFactorY);

                // Reset the starting point
                StartingPoint = currentPoint;
            }
        }

        #endregion

        private string _playfieldImagePath;
        private ImageSource _playfieldImage;
        private double _playfieldWidth;
        private double _playfieldHeight;
        private double _scaleFactorX;
        private double _scaleFactorY;
        private readonly IEventAggregator _eventAggregator;
        private double _playfieldToLedsScale;
        private bool _isPlayfieldVisible;
        private string _playfieldImageName;
        private string _playfieldImageLocation;
    }
}