﻿///////////////////////////////////////////////////////////////////////////////
//
// (C) 2015 ICE TEA GROUP LLC - ALL RIGHTS RESERVED
//
// 
//
// ALL INFORMATION CONTAINED HEREIN IS, AND REMAINS
// THE PROPERTY OF ICE TEA GROUP LLC AND ITS SUPPLIERS, IF ANY.
// THE INTELLECTUAL PROPERTY AND TECHNICAL CONCEPTS CONTAINED
// HEREIN ARE PROPRIETARY TO ICE TEA GROUP LLC AND ITS SUPPLIERS
// AND MAY BE COVERED BY U.S. AND FOREIGN PATENTS, PATENT IN PROCESS, AND
// ARE PROTECTED BY TRADE SECRET OR COPYRIGHT LAW.
//
// DISSEMINATION OF THIS INFORMATION OR REPRODUCTION OF THIS MATERIAL
// IS STRICTLY FORBIDDEN UNLESS PRIOR WRITTEN PERMISSION IS OBTAINED
// FROM ICE TEA GROUP LLC.
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Wisej.Core;
using Wisej.Core.Design;

namespace Wisej.Web.Ext.ChartJS
{
	/// <summary>
	/// ChartJS is a simple yet flexible JavaScript charting for designers and developers from http://www.chartjs.org/. 
	/// </summary>
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof(ChartJS))]
	[DefaultEvent("ChartClick")]
	[Description("ChartJS is a simple yet flexible JavaScript charting for designers & developers from http://www.chartjs.org/.")]
	public class ChartJS : Widget, IWisejControl
	{
		/// <summary>
		/// Constructs a new instance of the <see cref="T:Wisej.Web.Ext.ChartJS.ChartJS"/> control.
		/// </summary>
		public ChartJS()
		{
		}

		#region Events

		/// <summary>
		/// Fired when the user clicks a data point on the chart.
		/// </summary>
		[Description("Fired when the user clicks a data point on the chart.")]
		public event ChartClickEventHandler ChartClick
		{
			add { base.Events.AddHandler(nameof(ChartClick), value); }
			remove { base.Events.RemoveHandler(nameof(ChartClick), value); }
		}

		/// <summary>
		/// Fires the <see cref="E:Wisej.Web.Ext.ChartJS.ChartJS.ChartClick"/> event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnChartClick(ChartClickEventArgs e)
		{
			((ChartClickEventHandler)base.Events[nameof(ChartClick)])?.Invoke(this, e);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Returns or sets the <see cref="Wisej.Web.Ext.ChartJS.ChartType"/>.
		/// </summary>
		[DesignerActionList]
		[DefaultValue(ChartType.Line)]
		[RefreshProperties(RefreshProperties.All)]
		[Description("Returns or sets the chart type.")]
		public ChartType ChartType
		{
			get { return this._chartType; }
			set
			{
				if (this._chartType != value)
				{
					this._chartType = value;

					// change the options to match the chart type.
					this._options = CreateOptions();
					// change the data sets to match the chart type.
					this._dataSets = CreateDataSetCollection();

					Update();
				}
			}
		}
		private ChartType _chartType = ChartType.Line;

		/// <summary>
		/// Chart options specific for the value of <see cref="P:Wisej.Web.Ext.ChartJS.ChartJS.ChartType"/>.
		/// </summary>
		[MergableProperty(false)]
		[Description("Chart options specific for the value of the ChartType property.")]
		public new Options Options
		{
			get
			{
				if (this._options == null)
					this._options = CreateOptions();

				return this._options;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				value.Chart = this;
				this._options = value;

				Update();
			}
		}
		private Options _options = null;

		/// <summary>
		/// Returns or sets the data sets to plot the chart.
		/// </summary>
		[MergableProperty(false)]
		[Description("Returns or sets the data sets to plot the chart.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataSetCollection DataSets
		{
			get
			{
				if (this._dataSets == null)
					this._dataSets = CreateDataSetCollection();

				return this._dataSets;
			}
		}
		private DataSetCollection _dataSets = null;

		private bool ShouldSerializeDataSets()
		{
			return
				this._dataSets != null
				&& this._dataSets.Count > 0
				&& this._dataSets[0].Data != null
				&& this._dataSets[0].Data.Length > 0;
		}
		private void ResetDataSets()
		{
			this.Labels = null;
		}

		/// <summary>
		/// Returns or sets the labels for the data points.
		/// </summary>
		[DesignerActionList]
		[MergableProperty(false)]
		[TypeConverter(typeof(ArrayConverter))]
		[Editor("System.Windows.Forms.Design.StringArrayEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		public string[] Labels
		{
			get
			{
				return this._labels;
			}
			set
			{
				this._labels = value ?? new string[0];
				Update();
			}
		}
		private string[] _labels = new string[0];

		private bool ShouldSerializeLabels()
		{
			return this._labels != null && this._labels.Length > 0;
		}
		private void ResetLabels()
		{
			this.Labels = null;
		}

		/// <summary>
		/// Overridden to create our initialization script.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string InitScript
		{
			get { return BuildInitScript(); }
			set { }
		}

		// If we don't have a dataset and we are in design mode, return
		// a dummy dataset to match the labels just to draw something in the designer.
		private IList<DataSet> DesignDataSets
		{
			get
			{
				// create or re-create the design data set. we'll create only 1 at index 0.
				if (this._designDataSets == null
					|| this._designDataSets[0].Data.Length != this.Labels.Length
					|| this._designDataSets[0].Type != this.ChartType)
				{
					this._designDataSets = new DataSetCollection(this, null);
					DataSet dataSet = this._designDataSets.CreateDataSet("Sample DataSet");

					dataSet.Data = new object[this.Labels.Length];
					Random random = new Random();
					for (int i = 0; i < dataSet.Data.Length; i++)
						dataSet.Data[i] = random.Next(80);

					this._designDataSets.Add(dataSet);
				}
				return this._designDataSets;
			}
		}
		private DataSetCollection _designDataSets;


		#endregion

		#region Methods

		/// <summary>
		/// Causes the chart to update the data set and labels.
		/// It performs a smooth animated transition from one data set to the new one.
		/// </summary>
		/// <param name="duration">Duration of the update animation in milliseconds. The default is 300 milliseconds.</param>
		public void UpdateData(int duration = 300)
		{
			// if the control is already scheduled for a full update, there is no
			// point in updating the dataset since it will be fully redrawn with the next update.
			IWisejControl me = this;
			if (me.IsDirty)
				return;

			Call("update", this.DataSets, this.Labels, duration);
		}

		// Creates a new set of options.
		// Tries to preserve the shared properties from
		// the base class.
		private Options CreateOptions()
		{
			switch (this.ChartType)
			{
				case ChartType.Line:
					return new LineOptions(this, this._options);
				case ChartType.Bar:
				case ChartType.HorizontalBar:
					return new BarOptions(this, this._options);
				case ChartType.Pie:
					return new PieOptions(this, this._options);
				case ChartType.PolarArea:
					return new PolarAreaOptions(this, this._options);
				case ChartType.Doughnut:
					return new DoughnutOptions(this, this._options);
				case ChartType.Radar:
					return new RadarOptions(this, this._options);
				case ChartType.Bubble:
					return new BubbleOptions(this, this._options);
				case ChartType.Scatter:
					return new ScatterOptions(this, this._options);

				default:
					throw new InvalidOperationException("Unknown chart type.");
			}
		}

		// Creates a new data set collection.
		// Tries to preserve the existing data sets.
		private DataSetCollection CreateDataSetCollection()
		{
			return new DataSetCollection(this, this._dataSets);
		}

		#endregion

		#region Wisej Implementation

		/// <summary>
		/// Handles events fired by the widget.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnWidgetEvent(WidgetEventArgs e)
		{
			switch (e.Type)
			{
				case "chartClick":
					OnChartClick(new ChartClickEventArgs(this, e));
					break;

				default:
					base.OnWidgetEvent(e);
					break;
			}
		}

		/// <summary>
		/// Overridden to return our list of script resources.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override List<Package> Packages
		{
			get
			{
				if (base.Packages.Count == 0)
				{
					// initialize the loader with the required libraries.
					base.Packages.AddRange(new[] {
						new Package() {
							Name = "moment.js",
							Source = GetResourceURL("WiseJ.Web.Ext.ChartJs.JavaScript.moment-with-locales-2.17.1.js")
						},
						new Package()
						{
							Name = "chart.js",
							Source = GetResourceURL("WiseJ.Web.Ext.ChartJs.JavaScript.chart-2.7.2.js")
						},
						new Package()
						{
							Name = "dataLabelPlugin.js",
							Source = GetResourceURL("WiseJ.Web.Ext.ChartJs.JavaScript.dataLabelPlugin.js")
						}
					});
				}

				return base.Packages;
			}
		}

		private string BuildInitScript()
		{

			IWisejControl me = this;
			dynamic config = new DynamicObject();
			string script = GetResourceString("Wisej.Web.Ext.ChartJS.JavaScript.startup.js");

			config.type = this.ChartType;

			// the power of Wisej is all in the next two lines.
			// options and data are complex objects, with nested objects, arrays and
			// enumerations. we simply assign them to config and they will be picked
			// up by the javascript widget without any complex transformations.

			config.options = this.Options;

			config.data = new
			{
				labels = this.Labels,
				datasets =
					ShouldSerializeDataSets()
						? this.DataSets
						: this.DesignMode
							? this.DesignDataSets
							: null
			};

			script = script.Replace("$config", WisejSerializer.Serialize(config, WisejSerializerOptions.IgnoreNulls | WisejSerializerOptions.CamelCase));
			return script;
		}

		#endregion
	}
}
