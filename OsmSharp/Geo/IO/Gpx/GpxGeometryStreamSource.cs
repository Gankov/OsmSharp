﻿// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using OsmSharp.Geo.Attributes;
using OsmSharp.Geo.Geometries;
using OsmSharp.Geo.Streams;
using OsmSharp.Math.Geo;
using OsmSharp.Xml.Gpx;
using System.Collections.Generic;
using OsmSharp.Xml.Sources;

namespace OsmSharp.Geo.IO.Gpx
{
    /// <summary>
    /// Gpx-stream source.
    /// </summary>
    public class GpxGeometryStreamSource : GeoCollectionStreamSource
    {
        /// <summary>
        /// Holds the stream containing the source-data.
        /// </summary>
        private Stream _stream;

        /// <summary>
        /// Holds the flag to create invidiual points for each track position.
        /// </summary>
        private bool _createTrackPoints = true;

        /// <summary>
        /// Creates a new Gpx-geometry stream.
        /// </summary>
        /// <param name="stream"></param>
        public GpxGeometryStreamSource(Stream stream)
            : base(new GeometryCollection())
        {
            _stream = stream;
        }

        /// <summary>
        /// Creates a new Gpx-geometry stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="createTrackPoints"></param>
        public GpxGeometryStreamSource(Stream stream, bool createTrackPoints)
            : base(new GeometryCollection())
        {
            _stream = stream;
            _createTrackPoints = createTrackPoints;
        }

        /// <summary>
        /// Called when initializing this source.
        /// </summary>
        public override void Initialize()
        {
            // read the gpx-data.
            if (!_read) { this.DoReadGpx(); }

            base.Initialize();
        }

        #region Read Gpx

        /// <summary>
        /// The gpx object.
        /// </summary>
        private bool _read = false;

        /// <summary>
        /// Reads the actual Gpx.
        /// </summary>
        private void DoReadGpx()
        {            
            // seek to the beginning of the stream.
            if (_stream.CanSeek) { _stream.Seek(0, SeekOrigin.Begin); }

            // instantiate and load the gpx test document.
            XmlStreamSource source = new XmlStreamSource(_stream);
            GpxDocument document = new GpxDocument(source);
            object gpx = document.Gpx;

            switch(document.Version)
            {
                case GpxVersion.Gpxv1_0:
                    this.ReadGpxv1_0(gpx as OsmSharp.Xml.Gpx.v1_0.gpx);
                    break;
                case GpxVersion.Gpxv1_1:
                    this.ReadGpxv1_1(gpx as Xml.Gpx.v1_1.gpxType);
                    break;
            }
        }

        /// <summary>
        /// Reads a gpx v1.1 object into corresponding geometries.
        /// </summary>
        /// <param name="gpx"></param>
        private void ReadGpxv1_1(Xml.Gpx.v1_1.gpxType gpx)
        {
            this.GeometryCollection.Clear();

            // do the waypoints.
            if (gpx.wpt != null)
            { // there are waypoints.
                foreach (var wpt in gpx.wpt)
                {
                    Point point = new Point(
                        new GeoCoordinate((double)wpt.lat, (double)wpt.lon));
                    point.Attributes = new SimpleGeometryAttributeCollection();

                    if (wpt.ageofdgpsdataSpecified) { point.Attributes.Add("ageofdgpsdata", wpt.ageofdgpsdata); }
                    if (wpt.eleSpecified) { point.Attributes.Add("ele", wpt.ele); }
                    if (wpt.fixSpecified) { point.Attributes.Add("fix", wpt.fix); }
                    if (wpt.geoidheightSpecified) { point.Attributes.Add("geoidheight", wpt.geoidheight); }
                    if (wpt.hdopSpecified) { point.Attributes.Add("hdop", wpt.hdop); }
                    if (wpt.magvarSpecified) { point.Attributes.Add("magvar", wpt.magvar); }
                    if (wpt.pdopSpecified) { point.Attributes.Add("pdop", wpt.pdop); }
                    if (wpt.timeSpecified) { point.Attributes.Add("time", wpt.time); }
                    if (wpt.vdopSpecified) { point.Attributes.Add("vdop", wpt.vdop); }

                    if (wpt.cmt != null) { point.Attributes.Add("cmt", wpt.cmt); }
                    if (wpt.desc != null) { point.Attributes.Add("desc", wpt.desc); }
                    if (wpt.dgpsid != null) { point.Attributes.Add("dgpsid", wpt.dgpsid); }
                    if (wpt.name != null) { point.Attributes.Add("name", wpt.name); }
                    if (wpt.sat != null) { point.Attributes.Add("sat", wpt.sat); }
                    if (wpt.src != null) { point.Attributes.Add("src", wpt.src); }
                    if (wpt.sym != null) { point.Attributes.Add("sym", wpt.sym); }
                    if (wpt.type != null) { point.Attributes.Add("type", wpt.type); }

                    this.GeometryCollection.Add(point);
                }
            }

            // do the routes.
            if (gpx.rte != null)
            {
                foreach (var rte in gpx.rte)
                { // convert to a line-string
                    List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
                    foreach (var rtept in rte.rtept)
                    {
                        GeoCoordinate coordinate = new GeoCoordinate((double)rtept.lat, (double)rtept.lon);
                        coordinates.Add(coordinate);

                        if (_createTrackPoints)
                        {
                            Point point = new Point(coordinate);
                            point.Attributes = new SimpleGeometryAttributeCollection();

                            if (rtept.ageofdgpsdataSpecified) { point.Attributes.Add("ageofdgpsdata", rtept.ageofdgpsdata); }
                            if (rtept.eleSpecified) { point.Attributes.Add("ele", rtept.ele); }
                            if (rtept.fixSpecified) { point.Attributes.Add("fix", rtept.fix); }
                            if (rtept.geoidheightSpecified) { point.Attributes.Add("geoidheight", rtept.geoidheight); }
                            if (rtept.hdopSpecified) { point.Attributes.Add("hdop", rtept.hdop); }
                            if (rtept.magvarSpecified) { point.Attributes.Add("magvar", rtept.magvar); }
                            if (rtept.pdopSpecified) { point.Attributes.Add("pdop", rtept.pdop); }
                            if (rtept.timeSpecified) { point.Attributes.Add("time", rtept.time); }
                            if (rtept.vdopSpecified) { point.Attributes.Add("vdop", rtept.vdop); }

                            if (rtept.cmt != null) { point.Attributes.Add("cmt", rtept.cmt); }
                            if (rtept.desc != null) { point.Attributes.Add("desc", rtept.desc); }
                            if (rtept.dgpsid != null) { point.Attributes.Add("dgpsid", rtept.dgpsid); }
                            if (rtept.name != null) { point.Attributes.Add("name", rtept.name); }
                            if (rtept.sat != null) { point.Attributes.Add("sat", rtept.sat); }
                            if (rtept.src != null) { point.Attributes.Add("src", rtept.src); }
                            if (rtept.sym != null) { point.Attributes.Add("sym", rtept.sym); }
                            if (rtept.type != null) { point.Attributes.Add("type", rtept.type); }

                            this.GeometryCollection.Add(point);
                        }
                    }

                    // creates a new linestring.
                    LineString lineString = new LineString(coordinates);
                    lineString.Attributes = new SimpleGeometryAttributeCollection();
                    if (rte.cmt != null) { lineString.Attributes.Add("cmt", rte.cmt); }
                    if (rte.desc != null) { lineString.Attributes.Add("desc", rte.desc); }
                    if (rte.name != null) { lineString.Attributes.Add("name", rte.name); }
                    if (rte.number != null) { lineString.Attributes.Add("number", rte.number); }
                    if (rte.src != null) { lineString.Attributes.Add("src", rte.src); }

                    this.GeometryCollection.Add(lineString);
                }
            }

            // do the tracks.
            foreach (var trk in gpx.trk)
            { // convert to a line-string
                List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
                foreach (var trkseg in trk.trkseg)
                {
                    foreach (var wpt in trkseg.trkpt)
                    {
                        GeoCoordinate coordinate = new GeoCoordinate((double)wpt.lat, (double)wpt.lon);

                        // only add new coordinates.
                        if (coordinates.Count == 0 || coordinates[coordinates.Count - 1] != coordinate)
                        {
                            coordinates.Add(coordinate);
                        }

                        if (_createTrackPoints)
                        {
                            Point point = new Point(coordinate);
                            point.Attributes = new SimpleGeometryAttributeCollection();

                            if (wpt.ageofdgpsdataSpecified) { point.Attributes.Add("ageofdgpsdata", wpt.ageofdgpsdata); }
                            if (wpt.eleSpecified) { point.Attributes.Add("ele", wpt.ele); }
                            if (wpt.fixSpecified) { point.Attributes.Add("fix", wpt.fix); }
                            if (wpt.geoidheightSpecified) { point.Attributes.Add("geoidheight", wpt.geoidheight); }
                            if (wpt.hdopSpecified) { point.Attributes.Add("hdop", wpt.hdop); }
                            if (wpt.magvarSpecified) { point.Attributes.Add("magvar", wpt.magvar); }
                            if (wpt.pdopSpecified) { point.Attributes.Add("pdop", wpt.pdop); }
                            if (wpt.timeSpecified) { point.Attributes.Add("time", wpt.time); }
                            if (wpt.vdopSpecified) { point.Attributes.Add("vdop", wpt.vdop); }

                            if (wpt.cmt != null) { point.Attributes.Add("cmt", wpt.cmt); }
                            if (wpt.desc != null) { point.Attributes.Add("desc", wpt.desc); }
                            if (wpt.dgpsid != null) { point.Attributes.Add("dgpsid", wpt.dgpsid); }
                            if (wpt.name != null) { point.Attributes.Add("name", wpt.name); }
                            if (wpt.sat != null) { point.Attributes.Add("sat", wpt.sat); }
                            if (wpt.src != null) { point.Attributes.Add("src", wpt.src); }
                            if (wpt.sym != null) { point.Attributes.Add("sym", wpt.sym); }
                            if (wpt.type != null) { point.Attributes.Add("type", wpt.type); }

                            this.GeometryCollection.Add(point);
                        }
                    }
                }

                // creates a new linestring.
                LineString lineString = new LineString(coordinates);
                lineString.Attributes = new SimpleGeometryAttributeCollection();
                if (trk.cmt != null) { lineString.Attributes.Add("cmt", trk.cmt); }
                if (trk.desc != null) { lineString.Attributes.Add("desc", trk.desc); }
                if (trk.name != null) { lineString.Attributes.Add("name", trk.name); }
                if (trk.number != null) { lineString.Attributes.Add("number", trk.number); }
                if (trk.src != null) { lineString.Attributes.Add("src", trk.src); }

                this.GeometryCollection.Add(lineString);
            }
        }

        /// <summary>
        /// Reads a gpx v1.0 object into corresponding geometries.
        /// </summary>
        /// <param name="gpx"></param>
        private void ReadGpxv1_0(OsmSharp.Xml.Gpx.v1_0.gpx gpx)
        {
            this.GeometryCollection.Clear();

            // do the waypoints.
            if (gpx.wpt != null)
            { // there are waypoints.
                foreach (var wpt in gpx.wpt)
                {
                    Point point = new Point(
                        new GeoCoordinate((double)wpt.lat, (double)wpt.lon));
                    point.Attributes = new SimpleGeometryAttributeCollection();

                    if (wpt.ageofdgpsdataSpecified) { point.Attributes.Add("ageofdgpsdata", wpt.ageofdgpsdata); }
                    if (wpt.eleSpecified) { point.Attributes.Add("ele", wpt.ele); }
                    if (wpt.fixSpecified) { point.Attributes.Add("fix", wpt.fix); }
                    if (wpt.geoidheightSpecified) { point.Attributes.Add("geoidheight", wpt.geoidheight); }
                    if (wpt.hdopSpecified) { point.Attributes.Add("hdop", wpt.hdop); }
                    if (wpt.magvarSpecified) { point.Attributes.Add("magvar", wpt.magvar); }
                    if (wpt.pdopSpecified) { point.Attributes.Add("pdop", wpt.pdop); }
                    if (wpt.timeSpecified) { point.Attributes.Add("time", wpt.time); }
                    if (wpt.vdopSpecified) { point.Attributes.Add("vdop", wpt.vdop); }

                    if (wpt.Any != null) { point.Attributes.Add("Any", wpt.Any); }
                    if (wpt.cmt != null) { point.Attributes.Add("cmt", wpt.cmt); }
                    if (wpt.desc != null) { point.Attributes.Add("desc", wpt.desc); }
                    if (wpt.dgpsid != null) { point.Attributes.Add("dgpsid", wpt.dgpsid); }
                    if (wpt.name != null) { point.Attributes.Add("name", wpt.name); }
                    if (wpt.sat != null) { point.Attributes.Add("sat", wpt.sat); }
                    if (wpt.src != null) { point.Attributes.Add("src", wpt.src); }
                    if (wpt.sym != null) { point.Attributes.Add("sym", wpt.sym); }
                    if (wpt.url != null) { point.Attributes.Add("url", wpt.url); }
                    if (wpt.urlname != null) { point.Attributes.Add("urlname", wpt.urlname); }
                    if (wpt.type != null) { point.Attributes.Add("type", wpt.type); }

                    this.GeometryCollection.Add(point);
                }
            }

            // do the routes.
            if (gpx.rte != null)
            {
                foreach (var rte in gpx.rte)
                { // convert to a line-string
                    List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
                    foreach (var rtept in rte.rtept)
                    {
                        GeoCoordinate coordinate = new GeoCoordinate((double)rtept.lat, (double)rtept.lon);
                        coordinates.Add(coordinate);

                        if (_createTrackPoints)
                        {
                            Point point = new Point(coordinate);
                            point.Attributes = new SimpleGeometryAttributeCollection();

                            if (rtept.ageofdgpsdataSpecified) { point.Attributes.Add("ageofdgpsdata", rtept.ageofdgpsdata); }
                            if (rtept.eleSpecified) { point.Attributes.Add("ele", rtept.ele); }
                            if (rtept.fixSpecified) { point.Attributes.Add("fix", rtept.fix); }
                            if (rtept.geoidheightSpecified) { point.Attributes.Add("geoidheight", rtept.geoidheight); }
                            if (rtept.hdopSpecified) { point.Attributes.Add("hdop", rtept.hdop); }
                            if (rtept.magvarSpecified) { point.Attributes.Add("magvar", rtept.magvar); }
                            if (rtept.pdopSpecified) { point.Attributes.Add("pdop", rtept.pdop); }
                            if (rtept.timeSpecified) { point.Attributes.Add("time", rtept.time); }
                            if (rtept.vdopSpecified) { point.Attributes.Add("vdop", rtept.vdop); }

                            if (rtept.Any != null) { point.Attributes.Add("Any", rtept.Any); }
                            if (rtept.cmt != null) { point.Attributes.Add("cmt", rtept.cmt); }
                            if (rtept.desc != null) { point.Attributes.Add("desc", rtept.desc); }
                            if (rtept.dgpsid != null) { point.Attributes.Add("dgpsid", rtept.dgpsid); }
                            if (rtept.name != null) { point.Attributes.Add("name", rtept.name); }
                            if (rtept.sat != null) { point.Attributes.Add("sat", rtept.sat); }
                            if (rtept.src != null) { point.Attributes.Add("src", rtept.src); }
                            if (rtept.sym != null) { point.Attributes.Add("sym", rtept.sym); }
                            if (rtept.url != null) { point.Attributes.Add("url", rtept.url); }
                            if (rtept.urlname != null) { point.Attributes.Add("urlname", rtept.urlname); }
                            if (rtept.type != null) { point.Attributes.Add("type", rtept.type); }

                            this.GeometryCollection.Add(point);
                        }
                    }

                    // creates a new linestring.
                    LineString lineString = new LineString(coordinates);
                    lineString.Attributes = new SimpleGeometryAttributeCollection();
                    if (rte.Any != null) { lineString.Attributes.Add("Any", rte.Any); }
                    if (rte.cmt != null) { lineString.Attributes.Add("cmt", rte.cmt); }
                    if (rte.desc != null) { lineString.Attributes.Add("desc", rte.desc); }
                    if (rte.name != null) { lineString.Attributes.Add("name", rte.name); }
                    if (rte.number != null) { lineString.Attributes.Add("number", rte.number); }
                    if (rte.src != null) { lineString.Attributes.Add("src", rte.src); }
                    if (rte.url != null) { lineString.Attributes.Add("url", rte.url); }
                    if (rte.urlname != null) { lineString.Attributes.Add("urlname", rte.urlname); }

                    this.GeometryCollection.Add(lineString);
                }
            }

            // do the tracks.
            foreach (var trk in gpx.trk)
            { // convert to a line-string
                List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
                foreach (var trkseg in trk.trkseg)
                {
                    GeoCoordinate coordinate = new GeoCoordinate((double)trkseg.lat, (double)trkseg.lon);
                    coordinates.Add(coordinate);

                    if (_createTrackPoints)
                    {
                        Point point = new Point(coordinate);
                        point.Attributes = new SimpleGeometryAttributeCollection();

                        if (trkseg.ageofdgpsdataSpecified) { point.Attributes.Add("ageofdgpsdata", trkseg.ageofdgpsdata); }
                        if (trkseg.eleSpecified) { point.Attributes.Add("ele", trkseg.ele); }
                        if (trkseg.fixSpecified) { point.Attributes.Add("fix", trkseg.fix); }
                        if (trkseg.geoidheightSpecified) { point.Attributes.Add("geoidheight", trkseg.geoidheight); }
                        if (trkseg.hdopSpecified) { point.Attributes.Add("hdop", trkseg.hdop); }
                        if (trkseg.magvarSpecified) { point.Attributes.Add("magvar", trkseg.magvar); }
                        if (trkseg.pdopSpecified) { point.Attributes.Add("pdop", trkseg.pdop); }
                        if (trkseg.timeSpecified) { point.Attributes.Add("time", trkseg.time); }
                        if (trkseg.vdopSpecified) { point.Attributes.Add("vdop", trkseg.vdop); }

                        if (trkseg.Any != null) { point.Attributes.Add("Any", trkseg.Any); }
                        if (trkseg.cmt != null) { point.Attributes.Add("cmt", trkseg.cmt); }
                        if (trkseg.desc != null) { point.Attributes.Add("desc", trkseg.desc); }
                        if (trkseg.dgpsid != null) { point.Attributes.Add("dgpsid", trkseg.dgpsid); }
                        if (trkseg.name != null) { point.Attributes.Add("name", trkseg.name); }
                        if (trkseg.sat != null) { point.Attributes.Add("sat", trkseg.sat); }
                        if (trkseg.src != null) { point.Attributes.Add("src", trkseg.src); }
                        if (trkseg.sym != null) { point.Attributes.Add("sym", trkseg.sym); }
                        if (trkseg.url != null) { point.Attributes.Add("url", trkseg.url); }
                        if (trkseg.urlname != null) { point.Attributes.Add("urlname", trkseg.urlname); }
                        if (trkseg.type != null) { point.Attributes.Add("type", trkseg.type); }

                        this.GeometryCollection.Add(point);
                    }
                }

                // creates a new linestring.
                LineString lineString = new LineString(coordinates);
                lineString.Attributes = new SimpleGeometryAttributeCollection();
                if (trk.Any != null) { lineString.Attributes.Add("Any", trk.Any); }
                if (trk.cmt != null) { lineString.Attributes.Add("cmt", trk.cmt); }
                if (trk.desc != null) { lineString.Attributes.Add("desc", trk.desc); }
                if (trk.name != null) { lineString.Attributes.Add("name", trk.name); }
                if (trk.number != null) { lineString.Attributes.Add("number", trk.number); }
                if (trk.src != null) { lineString.Attributes.Add("src", trk.src); }
                if (trk.url != null) { lineString.Attributes.Add("url", trk.url); }
                if (trk.urlname != null) { lineString.Attributes.Add("urlname", trk.urlname); }

                this.GeometryCollection.Add(lineString);
            }
        }

        #endregion
    }
}
