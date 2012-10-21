﻿// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.TSPLIB.Problems;
using System.IO;

namespace Tools.TSPLIB
{
    class Program
    {
        static void Main(string[] args)
        {
            string file_name = "ft70.atsp";
            TSPLIBProblem problem = 
                TSPLIB.Parser.TSPLIBProblemParser.ParseFrom(file_name);
            TSPLIBProblem atsp = Convertor.ATSP_TSP.ATSP_TSPConvertor.Convert(problem);
            Parser.TSPLIBProblemGenerator.Generate(new FileInfo("output.tsp"),
                atsp);
        }
    }
}