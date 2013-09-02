﻿// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using NUnit.Framework;
using OsmSharp.Math.VRP.Core.Routes;
using OsmSharp.Math.VRP.Core.Routes.ASymmetric;
using OsmSharp.Routing.VRP.WithDepot.MaxTime;

namespace OsmSharp.UnitTests.Routes
{
    /// <summary>
    /// Tests the depot route wrapper.
    /// </summary>
    [TestFixture]
    public class DepotRouteTests
    {
        /// <summary>
        /// Tests an empty route.
        /// </summary>
        [Test]
        public void TestDepotDynamicAsymmetricRouteEmpty()
        {
            var route = new DepotRoute(
                new DynamicAsymmetricRoute(10, false));

            route.InsertAfter(0, 1);

            var customers = new List<int>(route);
            Assert.AreEqual(0, customers[0]);
            Assert.AreEqual(1, customers[1]);
        }

        /// <summary>
        /// Tests insert after on a depot route.
        /// </summary>
        [Test]
        public void TestDepotDynamicAsymmetricRouteInsertAfter()
        {
            var route = new DepotRoute(
                new DynamicAsymmetricRoute(10, false));

            route.InsertAfter(0, 1);

            var customers = new List<int>(route);
            Assert.AreEqual(0, customers[0]);
            Assert.AreEqual(1, customers[1]);

            route.InsertAfter(1, 2);

            customers = new List<int>(route);
            Assert.AreEqual(0, customers[0]);
            Assert.AreEqual(1, customers[1]);
            Assert.AreEqual(2, customers[2]);

            route.InsertAfter(1, 3);

            customers = new List<int>(route);
            Assert.AreEqual(0, customers[0]);
            Assert.AreEqual(1, customers[1]);
            Assert.AreEqual(3, customers[2]);
            Assert.AreEqual(2, customers[3]);

            route.InsertAfter(0, 4);

            customers = new List<int>(route);
            Assert.AreEqual(0, customers[0]);
            Assert.AreEqual(4, customers[1]);
            Assert.AreEqual(1, customers[2]);
            Assert.AreEqual(3, customers[3]);
            Assert.AreEqual(2, customers[4]);
        }

        /// <summary>
        /// Tests replace edge on a depot route.
        /// </summary>
        [Test]
        public void TestDepotDynamicAsymmetricRouteReplaceEdge()
        {
            var route = new DepotRoute(
                new DynamicAsymmetricRoute(10, false));
            route.InsertAfter(0, 1);
            route.InsertAfter(1, 2);
            route.InsertAfter(2, 3);
            route.InsertAfter(3, 4);

            var customers = new List<int>(route);
            Assert.AreEqual(5, customers.Count);
            Assert.AreEqual(0, customers[0]);
            Assert.AreEqual(1, customers[1]);
            Assert.AreEqual(2, customers[2]);
            Assert.AreEqual(3, customers[3]);
            Assert.AreEqual(4, customers[4]);
            //route.ReplaceEdgeFrom(0, 1);
            Assert.IsTrue(route.IsValid());

            route.ReplaceEdgeFrom(2, 4);
            route.ReplaceEdgeFrom(4, 3);
            route.ReplaceEdgeFrom(3, 0);

            customers = new List<int>(route);
            Assert.AreEqual(5, customers.Count);
            Assert.AreEqual(0, customers[0]);
            Assert.AreEqual(1, customers[1]);
            Assert.AreEqual(2, customers[2]);
            Assert.AreEqual(4, customers[3]);
            Assert.AreEqual(3, customers[4]);
            Assert.IsTrue(route.IsValid());

            route.ReplaceEdgeFrom(0, 0);
            customers = new List<int>(route);
            Assert.AreEqual(1, customers.Count);
            Assert.AreEqual(0, customers[0]);

            Assert.IsTrue(route.IsValid());
        }

        /// <summary>
        /// Regression test of the multi dynamic asymetric depot wrapper simulating an exchange of parts of the routes.
        /// </summary>
        [Test]
        public void TestDepotDynamicAsymmetricMultiRouteExchanges()
        {
            // create two routes.
            // 0->11->12->13->14->15->0
            // 0->21->22->23->24->25->0
            var multiRoute = new MaxTimeSolution(0);
            IRoute route1 = multiRoute.Add();
            var customers = new List<int>(route1);
            Assert.AreEqual(1, customers.Count);
            Assert.AreEqual(0, customers[0]);
            route1.InsertAfter(0, 11);
            Assert.AreEqual("0->11", route1.ToString());
            route1.InsertAfter(11, 12);
            route1.InsertAfter(12, 13);
            route1.InsertAfter(13, 14);
            route1.InsertAfter(14, 15);
            IRoute route2 = multiRoute.Add();
            customers = new List<int>(route2);
            Assert.AreEqual(1, customers.Count);
            Assert.AreEqual(0, customers[0]);
            route2.InsertAfter(0, 21);
            Assert.AreEqual("0->21", route2.ToString());
            route2.InsertAfter(21, 22);
            route2.InsertAfter(22, 23);
            route2.InsertAfter(23, 24);
            route2.InsertAfter(24, 25);

            customers = new List<int>(route1);
            Assert.AreEqual(6, customers.Count);
            Assert.AreEqual(0, customers[0]);
            Assert.AreEqual(11, customers[1]);
            Assert.AreEqual(12, customers[2]);
            Assert.AreEqual(13, customers[3]);
            Assert.AreEqual(14, customers[4]);
            Assert.AreEqual(15, customers[5]);


            customers = new List<int>(route2);
            Assert.AreEqual(6, customers.Count);
            Assert.AreEqual(0, customers[0]);
            Assert.AreEqual(21, customers[1]);
            Assert.AreEqual(22, customers[2]);
            Assert.AreEqual(23, customers[3]);
            Assert.AreEqual(24, customers[4]);
            Assert.AreEqual(25, customers[5]);

            // replace the entire first route.
            route1.ReplaceEdgeFrom(0, 0);

            route2.ReplaceEdgeFrom(25, 11);
            route2.ReplaceEdgeFrom(11, 12);
            route2.ReplaceEdgeFrom(12, 13);
            route2.ReplaceEdgeFrom(13, 14);
            route2.ReplaceEdgeFrom(14, 15);

            Assert.IsTrue(multiRoute.IsValid());

            // create two routes.
            // 0->11->12->13->14->15->0
            // 0->21->22->23->24->25->0
            multiRoute = new MaxTimeSolution(0);
            route1 = multiRoute.Add();
            customers = new List<int>(route1);
            Assert.AreEqual(1, customers.Count);
            Assert.AreEqual(0, customers[0]);
            route1.InsertAfter(0, 11);
            Assert.AreEqual("0->11", route1.ToString());
            route1.InsertAfter(11, 12);
            route1.InsertAfter(12, 13);
            route1.InsertAfter(13, 14);
            route1.InsertAfter(14, 15);
            route2 = multiRoute.Add();
            customers = new List<int>(route2);
            Assert.AreEqual(1, customers.Count);
            Assert.AreEqual(0, customers[0]);
            route2.InsertAfter(0, 21);
            Assert.AreEqual("0->21", route2.ToString());
            route2.InsertAfter(21, 22);
            route2.InsertAfter(22, 23);
            route2.InsertAfter(23, 24);
            route2.InsertAfter(24, 25);

            // exchange parts.
            var part1 = new List<int>(route1.Between(11, 13));
            var part2 = new List<int>(route2.Between(23, 25));
            route1.ReplaceEdgeFrom(0, 14);
            route2.ReplaceEdgeFrom(22, 0);

            int previous = 0;
            for (int idx = 0; idx < part2.Count; idx++)
            {
                route1.ReplaceEdgeFrom(previous, part2[idx]);
                previous = part2[idx];
            }
            route1.ReplaceEdgeFrom(previous, 14);

            previous = 22;
            for (int idx = 0; idx < part1.Count; idx++)
            {
                route2.ReplaceEdgeFrom(previous, part1[idx]);
                previous = part1[idx];
            }
            route2.ReplaceEdgeFrom(previous, 0);

            Assert.IsTrue(multiRoute.IsValid());

            customers = new List<int>(route1);
            Assert.AreEqual(6, customers.Count);
            Assert.AreEqual(0, customers[0]);
            Assert.AreEqual(23, customers[1]);
            Assert.AreEqual(24, customers[2]);
            Assert.AreEqual(25, customers[3]);
            Assert.AreEqual(14, customers[4]);
            Assert.AreEqual(15, customers[5]);

            customers = new List<int>(route2);
            Assert.AreEqual(6, customers.Count);
            Assert.AreEqual(0, customers[0]);
            Assert.AreEqual(21, customers[1]);
            Assert.AreEqual(22, customers[2]);
            Assert.AreEqual(11, customers[3]);
            Assert.AreEqual(12, customers[4]);
            Assert.AreEqual(13, customers[5]);

            // create two routes.
            // 0->11->12->13->14->15->0
            // 0->21->22->23->24->25->0
            multiRoute = new MaxTimeSolution(0);
            route1 = multiRoute.Add();
            customers = new List<int>(route1);
            Assert.AreEqual(1, customers.Count);
            Assert.AreEqual(0, customers[0]);
            route1.InsertAfter(0, 11);
            Assert.AreEqual("0->11", route1.ToString());
            route1.InsertAfter(11, 12);
            route1.InsertAfter(12, 13);
            route1.InsertAfter(13, 14);
            route1.InsertAfter(14, 15);
            route2 = multiRoute.Add();
            customers = new List<int>(route2);
            Assert.AreEqual(1, customers.Count);
            Assert.AreEqual(0, customers[0]);
            route2.InsertAfter(0, 21);
            Assert.AreEqual("0->21", route2.ToString());
            route2.InsertAfter(21, 22);
            route2.InsertAfter(22, 23);
            route2.InsertAfter(23, 24);
            route2.InsertAfter(24, 25);

            route1.ReplaceEdgeFrom(12, 15);
            route1.ReplaceEdgeFrom(14, 11);
            route1.ReplaceEdgeFrom(0, 13);

            customers = new List<int>(route1);
            Assert.AreEqual(6, customers.Count);
            Assert.AreEqual(0, customers[0]);
            Assert.AreEqual(13, customers[1]);
            Assert.AreEqual(14, customers[2]);
            Assert.AreEqual(11, customers[3]);
            Assert.AreEqual(12, customers[4]);
            Assert.AreEqual(15, customers[5]);

            route1.ToString();
        }
    }
}
