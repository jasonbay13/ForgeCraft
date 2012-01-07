/*
	Copyright 2011 ForgeCraft team
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SMP.PLAYER;

namespace SMP.GUI
{
	public partial class InventoryDebug : Form
	{
		public InventoryDebug()
		{
			InitializeComponent();
		}
		public void refreshz()
		{
			Player p = Player.FindPlayer(Username.Text);
			i0.Text = String.Format("{0}-{1}-{2}", p.inventory.items[0].id, p.inventory.items[0].count, p.inventory.items[0].meta);
			i1.Text = String.Format("{0}-{1}-{2}", p.inventory.items[1].id, p.inventory.items[1].count, p.inventory.items[1].meta);
			i2.Text = String.Format("{0}-{1}-{2}", p.inventory.items[2].id, p.inventory.items[2].count, p.inventory.items[2].meta);
			i3.Text = String.Format("{0}-{1}-{2}", p.inventory.items[3].id, p.inventory.items[3].count, p.inventory.items[3].meta);
			i4.Text = String.Format("{0}-{1}-{2}", p.inventory.items[4].id, p.inventory.items[4].count, p.inventory.items[4].meta);
			i5.Text = String.Format("{0}-{1}-{2}", p.inventory.items[5].id, p.inventory.items[5].count, p.inventory.items[5].meta);
			i6.Text = String.Format("{0}-{1}-{2}", p.inventory.items[6].id, p.inventory.items[6].count, p.inventory.items[6].meta);
			i7.Text = String.Format("{0}-{1}-{2}", p.inventory.items[7].id, p.inventory.items[7].count, p.inventory.items[7].meta);
			i8.Text = String.Format("{0}-{1}-{2}", p.inventory.items[8].id, p.inventory.items[8].count, p.inventory.items[8].meta);
			i9.Text = String.Format("{0}-{1}-{2}", p.inventory.items[9].id, p.inventory.items[9].count, p.inventory.items[9].meta);
			i10.Text = String.Format("{0}-{1}-{2}", p.inventory.items[10].id, p.inventory.items[10].count, p.inventory.items[10].meta);
			i11.Text = String.Format("{0}-{1}-{2}", p.inventory.items[11].id, p.inventory.items[11].count, p.inventory.items[11].meta);
			i12.Text = String.Format("{0}-{1}-{2}", p.inventory.items[12].id, p.inventory.items[12].count, p.inventory.items[12].meta);
			i13.Text = String.Format("{0}-{1}-{2}", p.inventory.items[13].id, p.inventory.items[13].count, p.inventory.items[13].meta);
			i14.Text = String.Format("{0}-{1}-{2}", p.inventory.items[14].id, p.inventory.items[14].count, p.inventory.items[14].meta);
			i15.Text = String.Format("{0}-{1}-{2}", p.inventory.items[15].id, p.inventory.items[15].count, p.inventory.items[15].meta);
			i16.Text = String.Format("{0}-{1}-{2}", p.inventory.items[16].id, p.inventory.items[16].count, p.inventory.items[16].meta);
			i17.Text = String.Format("{0}-{1}-{2}", p.inventory.items[17].id, p.inventory.items[17].count, p.inventory.items[17].meta);
			i18.Text = String.Format("{0}-{1}-{2}", p.inventory.items[18].id, p.inventory.items[18].count, p.inventory.items[18].meta);
			i19.Text = String.Format("{0}-{1}-{2}", p.inventory.items[19].id, p.inventory.items[19].count, p.inventory.items[19].meta);
			i20.Text = String.Format("{0}-{1}-{2}", p.inventory.items[20].id, p.inventory.items[20].count, p.inventory.items[20].meta);
			i21.Text = String.Format("{0}-{1}-{2}", p.inventory.items[21].id, p.inventory.items[21].count, p.inventory.items[21].meta);
			i22.Text = String.Format("{0}-{1}-{2}", p.inventory.items[22].id, p.inventory.items[22].count, p.inventory.items[22].meta);
			i23.Text = String.Format("{0}-{1}-{2}", p.inventory.items[23].id, p.inventory.items[23].count, p.inventory.items[23].meta);
			i24.Text = String.Format("{0}-{1}-{2}", p.inventory.items[24].id, p.inventory.items[24].count, p.inventory.items[24].meta);
			i25.Text = String.Format("{0}-{1}-{2}", p.inventory.items[25].id, p.inventory.items[25].count, p.inventory.items[25].meta);
			i26.Text = String.Format("{0}-{1}-{2}", p.inventory.items[26].id, p.inventory.items[26].count, p.inventory.items[26].meta);
			i27.Text = String.Format("{0}-{1}-{2}", p.inventory.items[27].id, p.inventory.items[27].count, p.inventory.items[27].meta);
			i28.Text = String.Format("{0}-{1}-{2}", p.inventory.items[28].id, p.inventory.items[28].count, p.inventory.items[28].meta);
			i29.Text = String.Format("{0}-{1}-{2}", p.inventory.items[29].id, p.inventory.items[29].count, p.inventory.items[29].meta);
			i30.Text = String.Format("{0}-{1}-{2}", p.inventory.items[30].id, p.inventory.items[30].count, p.inventory.items[30].meta);
			i31.Text = String.Format("{0}-{1}-{2}", p.inventory.items[31].id, p.inventory.items[31].count, p.inventory.items[31].meta);
			i32.Text = String.Format("{0}-{1}-{2}", p.inventory.items[32].id, p.inventory.items[32].count, p.inventory.items[32].meta);
			i33.Text = String.Format("{0}-{1}-{2}", p.inventory.items[33].id, p.inventory.items[33].count, p.inventory.items[33].meta);
			i34.Text = String.Format("{0}-{1}-{2}", p.inventory.items[34].id, p.inventory.items[34].count, p.inventory.items[34].meta);
			i35.Text = String.Format("{0}-{1}-{2}", p.inventory.items[35].id, p.inventory.items[35].count, p.inventory.items[35].meta);
			i36.Text = String.Format("{0}-{1}-{2}", p.inventory.items[36].id, p.inventory.items[36].count, p.inventory.items[36].meta);
			i37.Text = String.Format("{0}-{1}-{2}", p.inventory.items[37].id, p.inventory.items[37].count, p.inventory.items[37].meta);
			i38.Text = String.Format("{0}-{1}-{2}", p.inventory.items[38].id, p.inventory.items[38].count, p.inventory.items[38].meta);
			i39.Text = String.Format("{0}-{1}-{2}", p.inventory.items[39].id, p.inventory.items[39].count, p.inventory.items[39].meta);
			i40.Text = String.Format("{0}-{1}-{2}", p.inventory.items[40].id, p.inventory.items[40].count, p.inventory.items[40].meta);
			i41.Text = String.Format("{0}-{1}-{2}", p.inventory.items[41].id, p.inventory.items[41].count, p.inventory.items[41].meta);
			i42.Text = String.Format("{0}-{1}-{2}", p.inventory.items[42].id, p.inventory.items[42].count, p.inventory.items[42].meta);
			i43.Text = String.Format("{0}-{1}-{2}", p.inventory.items[43].id, p.inventory.items[43].count, p.inventory.items[43].meta);
			i44.Text = String.Format("{0}-{1}-{2}", p.inventory.items[44].id, p.inventory.items[44].count, p.inventory.items[44].meta);

			OnMouse.Text = String.Format("{0}-{1}-{2}", p.OnMouse.id, p.OnMouse.count, p.OnMouse.meta);

		}

		private void button1_Click(object sender, EventArgs e)
		{
			refreshz();
		}

        private void InventoryDebug_Load(object sender, EventArgs e)
        {

        }
	}
}
