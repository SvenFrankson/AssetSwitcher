using UnityEngine;
using System.Collections;
using CreativityZero.Configuration;

namespace CreativityZero.Skinning {

	public class SkinningConf : AbstractConfiguration {

		public bool loadFromRemoteLocation;
		public string url;

		protected override void doUpdateConfiguration (AbstractConfiguration newConfiguration) {
			SkinningConf newConf = newConfiguration as SkinningConf;

			this.loadFromRemoteLocation = newConf.loadFromRemoteLocation;
			this.url = newConf.url;
		}
	}
}