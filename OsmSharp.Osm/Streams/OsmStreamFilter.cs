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

using OsmSharp.Osm;

namespace OsmSharp.Osm.Data.Streams
{
    /// <summary>
    /// An OSM stream filter.
    /// </summary>
    public abstract class OsmStreamFilter : OsmStreamSource
    {
        /// <summary>
        /// Holds the reader.
        /// </summary>
        private OsmStreamSource _reader;

        /// <summary>
        /// Creates a new OSM filter.
        /// </summary>
        public OsmStreamFilter()
        {

        }

        /// <summary>
        /// Registers a reader as the source to filter.
        /// </summary>
        /// <param name="source"></param>
        public virtual void RegisterSource(OsmStreamSource source)
        {
            _reader = source;
        }

        /// <summary>
        /// Returns the reader being filtered.
        /// </summary>
        protected OsmStreamSource Reader
        {
            get
            {
                return _reader;
            }
        }

        /// <summary>
        /// Initializes this filter.
        /// </summary>
        public abstract override void Initialize();

        /// <summary>
        /// Moves to the next object.
        /// </summary>
        /// <returns></returns>
        public abstract override bool MoveNext();

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public abstract override OsmGeo Current();
    }
}