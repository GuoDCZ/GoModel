﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.BaseClasses
{
	public abstract class ExecutableBase
	{
		public abstract MoveInfo ExecuteModule(GameModelBase gameModel, ArgBase? arg);
	}
}
