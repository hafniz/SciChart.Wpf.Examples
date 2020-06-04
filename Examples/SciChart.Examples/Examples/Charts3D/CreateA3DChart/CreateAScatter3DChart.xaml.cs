// *************************************************************************************
// SCICHART® Copyright SciChart Ltd. 2011-2020. All rights reserved.
//  
// Web: http://www.scichart.com
//   Support: support@scichart.com
//   Sales:   sales@scichart.com
// 
// CreateAScatter3DChart.xaml.cs is part of the SCICHART® Examples. Permission is hereby granted
// to modify, create derivative works, distribute and publish any part of this source
// code whether for commercial, private or personal use. 
// 
// The SCICHART® examples are distributed in the hope that they will be useful, but
// without any warranty. It is provided "AS IS" without warranty of any kind, either
// expressed or implied. 
// *************************************************************************************

using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SciChart.Charting3D;
using SciChart.Charting3D.Model;
using SciChart.Charting3D.PointMarkers;
using SciChart.Examples.Examples.Charts3D.Customize3DChart;

namespace SciChart.Examples.Examples.Charts3D.CreateA3DChart
{
    public partial class CreateAScatter3DChart : UserControl
    {
        TextSceneEntity currentTitle;

        public CreateAScatter3DChart()
        {
            InitializeComponent();

            PointMarkerCombo.Items.Add(typeof(SpherePointMarker3D));
            PointMarkerCombo.Items.Add(typeof(CubePointMarker3D));
            PointMarkerCombo.Items.Add(typeof(PyramidPointMarker3D));
            PointMarkerCombo.Items.Add(typeof(CylinderPointMarker3D));

            PlotSourceCombo.ItemsSource = Directory.EnumerateFiles("..\\ScatterPlotSources").Where(s => s.ToUpper().EndsWith(".CSV")).Select(s => Path.GetFileNameWithoutExtension(s));

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            currentTitle = new TextSceneEntity("Select a source from the 'PLOT' side menu.", Color.FromRgb(255, 255, 255), new Vector3(0, 300, 0), TextDisplayMode.FacingCameraAlways, 12, "Segoe UI");
            SciChart.Viewport3D.RootEntity.Children.Add(currentTitle);

            ScatterSeries3D.DataSeries = new XyzDataSeries3D<double>();
            PointMarkerCombo.SelectedIndex = 0;
        }

        private void PlotSourceCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedPlot = PlotSourceCombo.SelectedItem.ToString();
            SciChart.Viewport3D.RootEntity.Children.Remove(currentTitle);
            currentTitle = new TextSceneEntity(selectedPlot, Color.FromRgb(255, 255, 255), new Vector3(0, 300, 0), TextDisplayMode.FacingCameraAlways, 12, "Segoe UI");
            SciChart.Viewport3D.RootEntity.Children.Add(currentTitle);

            ScatterSeries3D.DataSeries = selectedPlot.ToUpper().EndsWith("-CUSTOMRGB") ? GetDataSeriesFromFileWithCustomRgb(selectedPlot) : GetDataSeriesFromFile(selectedPlot);
        }

        /// <summary>
        /// Expected header: datasetName, x, y, z, ..., r, g, b
        /// </summary>
        private XyzDataSeries3D<double> GetDataSeriesFromFileWithCustomRgb(string basename)
        {
            XyzDataSeries3D<double> dataSeries = new XyzDataSeries3D<double>();
            foreach (string line in File.ReadAllLines($"..\\ScatterPlotSources\\{basename}.csv"))
            {
                if (line.StartsWith("datasetName"))
                {
                    continue;
                }
                string[] fields = line.Split(',');
                double knnallrew = double.Parse(fields[1]);
                double nbpkid = double.Parse(fields[2]);
                double dtc44 = double.Parse(fields[3]);
                byte r = byte.Parse(fields[fields.Length - 3]);
                byte g = byte.Parse(fields[fields.Length - 2]);
                byte b = byte.Parse(fields[fields.Length - 1]);
                Color color = Color.FromRgb(r, g, b);
                dataSeries.Append(knnallrew, nbpkid, dtc44, new PointMetadata3D(color));
            }

            return dataSeries;
        }

        /// <summary>
        /// Expected header: datasetName, x, y, z, ..., category
        /// </summary>
        private XyzDataSeries3D<double> GetDataSeriesFromFile(string basename)
        {
            Color customGreen = Color.FromRgb(149, 211, 165);
            Color customYellow = Color.FromRgb(255, 192, 0);
            Color customRed = Color.FromRgb(248, 105, 107);
            Color customGray = Color.FromRgb(128, 128, 128);

            XyzDataSeries3D<double> dataSeries = new XyzDataSeries3D<double>();
            foreach (string line in File.ReadAllLines($"..\\ScatterPlotSources\\{basename}.csv"))
            {
                if (line.StartsWith("datasetName"))
                {
                    continue;
                }
                string[] fields = line.Split(',');
                double knnallrew = double.Parse(fields[1]);
                double nbpkid = double.Parse(fields[2]);
                double dtc44 = double.Parse(fields[3]);
                string category = fields[fields.Length - 1];
                Color color;
                if (basename.ToUpper().EndsWith("-TRUTH"))
                {
                    switch (category)
                    {
                        case "C":
                            color = customGreen;
                            break;
                        case "P":
                            color = customYellow;
                            break;
                        case "I":
                            color = customRed;
                            break;
                        default:
                            color = customGray;
                            break;
                    }
                }
                else
                {
                    switch (category)
                    {
                        case "m1Better":
                            color = customRed;
                            break;
                        case "m2Better":
                            color = customGreen;
                            break;
                        case "m3Better":
                            color = customYellow;
                            break;
                        default:
                            color = customGray;
                            break;
                    }
                }
                dataSeries.Append(knnallrew, nbpkid, dtc44, new PointMetadata3D(color));
            }

            return dataSeries;
        }

        private void PointMarkerCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ScatterSeries3D != null && OpacitySlider != null && SizeSlider != null)
            {
                ScatterSeries3D.PointMarker = (BasePointMarker3D)Activator.CreateInstance((Type)((ComboBox)sender).SelectedItem);
                ScatterSeries3D.PointMarker.Fill = Colors.LimeGreen;
                ScatterSeries3D.PointMarker.Size = (float)SizeSlider.Value;
                ScatterSeries3D.PointMarker.Opacity = OpacitySlider.Value;
            }
        }

        private void SizeSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ScatterSeries3D != null && ScatterSeries3D.PointMarker != null)
                ScatterSeries3D.PointMarker.Size = (float)((Slider)sender).Value;
        }

        private void OpacitySlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ScatterSeries3D != null && ScatterSeries3D.PointMarker != null)
                ScatterSeries3D.PointMarker.Opacity = ((Slider)sender).Value;
        }
    }
}