﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.MVVMModels
{
	public class SwitchModel:DiagramElementModel
	{
		public override string ClassName => SwitchModelClassName;
		public Point CanvasPos { get; set; }
		public string SwitchModuleName { get; set; } = "";
		public string SwitchTargetText { get; set; } = "";
		public string SwitchModelClassName { get; set; } = "";
		public List<CaseModel> CaseModels { get; set; } = new();
		public SwitchModel(Point canvasPos)
		{
			CanvasPos = canvasPos;
		}
		public SwitchModel() { }
	}
}
