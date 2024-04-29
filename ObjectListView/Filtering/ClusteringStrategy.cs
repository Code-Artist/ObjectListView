﻿/*
 * ClusteringStrategy - Implements a simple clustering strategy
 *
 * Author: Phillip Piper
 * Date: 3-March-2011 10:53 pm
 *
 * Change log:
 * 2011-03-03  JPP  - First version
 * 
 * Copyright (C) 2011-2014 Phillip Piper
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * If you wish to use this code in a closed source application, please contact phillip.piper@gmail.com.
 */

using System;
using System.Collections;
using System.Text;

namespace BrightIdeasSoftware
{

	/// <summary>
	/// This class provides a useful base implementation of a clustering
	/// strategy where the clusters are grouped around the value of a given column.
	/// </summary>
	public class ClusteringStrategy : IClusteringStrategy
	{

		#region Static properties

		/// <summary>This field is the text that will be shown to the user when a cluster key is null.</summary>
		/// <remarks>It is exposed so it can be localized.</remarks>
		public static readonly String NULL_LABEL = "[null]";

		/// <summary>This field is the text that will be shown to the user when a cluster key is empty (i.e. a String of zero length).</summary>
		/// <remarks>It is exposed so it can be localized.</remarks>
		public static readonly String EMPTY_LABEL = "[empty]";

		/// <summary>
		/// Gets or sets the format that will be used by default for clusters that only
		/// contain 1 item. The format String must accept two placeholders:
		/// - {0} is the cluster key converted to a String
		/// - {1} is the number of items in the cluster (always 1 in this case)
		/// </summary>
		public static String DefaultDisplayLabelFormatSingular { get; set; } = "{0} ({1} item)";

		/// <summary>
		/// Gets or sets the format that will be used by default for clusters that 
		/// contain 0 or two or more items. The format String must accept two placeholders:
		/// - {0} is the cluster key converted to a String
		/// - {1} is the number of items in the cluster
		/// </summary>
		public static String DefaultDisplayLabelFormatPlural { get; set; } = "{0} ({1} items)";

		#endregion

		#region Life and death

		/// <summary>Create a clustering strategy</summary>
		public ClusteringStrategy()
		{
			this.DisplayLabelFormatSingular = DefaultDisplayLabelFormatSingular;
			this.DisplayLabelFormatPlural = DefaultDisplayLabelFormatPlural;
		}

		#endregion

		#region Public properties

		/// <summary>Gets or sets the column upon which this strategy is operating</summary>
		public OLVColumn Column { get; set; }

		/// <summary>
		/// Gets or sets the format that will be used when the cluster
		/// contains only 1 item. The format String must accept two placeholders:
		/// - {0} is the cluster key converted to a String
		/// - {1} is the number of items in the cluster (always 1 in this case)
		/// </summary>
		/// <remarks>If this is not set, the value from 
		/// ClusteringStrategy.DefaultDisplayLabelFormatSingular will be used</remarks>
		public String DisplayLabelFormatSingular { get; set; }

		/// <summary>
		/// Gets or sets the format that will be used when the cluster 
		/// contains 0 or two or more items. The format String must accept two placeholders:
		/// - {0} is the cluster key converted to a String
		/// - {1} is the number of items in the cluster
		/// </summary>
		/// <remarks>If this is not set, the value from ClusteringStrategy.DefaultDisplayLabelFormatPlural will be used</remarks>
		public String DisplayLabelFormatPlural { get; set; }

		#endregion

		#region ICluster implementation

		/// <summary>Get the cluster key by which the given model will be partitioned by this strategy</summary>
		/// <param name="model"></param>
		/// <returns></returns>
		virtual public Object GetClusterKey(Object model)
			=> this.Column.GetValue(model);

		/// <summary>Create a cluster to hold the given cluster key</summary>
		/// <param name="clusterKey"></param>
		/// <returns></returns>
		virtual public ICluster CreateCluster(Object clusterKey)
			=> new Cluster(clusterKey);

		/// <summary>Gets the display label that the given cluster should use</summary>
		/// <param name="cluster"></param>
		/// <returns></returns>
		virtual public String GetClusterDisplayLabel(ICluster cluster)
		{
			String s = this.Column.ValueToString(cluster.ClusterKey) ?? NULL_LABEL;
			if(String.IsNullOrEmpty(s))
				s = EMPTY_LABEL;
			return this.ApplyDisplayFormat(cluster, s);
		}

		/// <summary>Create a filter that will include only model objects that match one or more of the given values.</summary>
		/// <param name="valuesChosenForFiltering"></param>
		/// <returns></returns>
		virtual public IModelFilter CreateFilter(IList valuesChosenForFiltering)
			=> new OneOfFilter(this.GetClusterKey, valuesChosenForFiltering);

		/// <summary>
		/// Create a label that combines the String representation of the cluster
		/// key with a format String that holds an "X [N items in cluster]" type layout.
		/// </summary>
		/// <param name="cluster"></param>
		/// <param name="s"></param>
		/// <returns></returns>
		virtual protected String ApplyDisplayFormat(ICluster cluster, String s)
		{
			String format = (cluster.Count == 1) ? this.DisplayLabelFormatSingular : this.DisplayLabelFormatPlural;
			return String.IsNullOrEmpty(format) ? s : String.Format(format, s, cluster.Count);
		}

		#endregion
	}
}