using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
			i0.Text = String.Format("{0}-{1}-{2}", p.inventory.items[0].item, p.inventory.items[0].count, p.inventory.items[0].meta);
			i1.Text = String.Format("{0}-{1}-{2}", p.inventory.items[1].item, p.inventory.items[1].count, p.inventory.items[1].meta);
			i2.Text = String.Format("{0}-{1}-{2}", p.inventory.items[2].item, p.inventory.items[2].count, p.inventory.items[2].meta);
			i3.Text = String.Format("{0}-{1}-{2}", p.inventory.items[3].item, p.inventory.items[3].count, p.inventory.items[3].meta);
			i4.Text = String.Format("{0}-{1}-{2}", p.inventory.items[4].item, p.inventory.items[4].count, p.inventory.items[4].meta);
			i5.Text = String.Format("{0}-{1}-{2}", p.inventory.items[5].item, p.inventory.items[5].count, p.inventory.items[5].meta);
			i6.Text = String.Format("{0}-{1}-{2}", p.inventory.items[6].item, p.inventory.items[6].count, p.inventory.items[6].meta);
			i7.Text = String.Format("{0}-{1}-{2}", p.inventory.items[7].item, p.inventory.items[7].count, p.inventory.items[7].meta);
			i8.Text = String.Format("{0}-{1}-{2}", p.inventory.items[8].item, p.inventory.items[8].count, p.inventory.items[8].meta);
			i9.Text = String.Format("{0}-{1}-{2}", p.inventory.items[9].item, p.inventory.items[9].count, p.inventory.items[9].meta);
			i10.Text = String.Format("{0}-{1}-{2}", p.inventory.items[10].item, p.inventory.items[10].count, p.inventory.items[10].meta);
			i11.Text = String.Format("{0}-{1}-{2}", p.inventory.items[11].item, p.inventory.items[11].count, p.inventory.items[11].meta);
			i12.Text = String.Format("{0}-{1}-{2}", p.inventory.items[12].item, p.inventory.items[12].count, p.inventory.items[12].meta);
			i13.Text = String.Format("{0}-{1}-{2}", p.inventory.items[13].item, p.inventory.items[13].count, p.inventory.items[13].meta);
			i14.Text = String.Format("{0}-{1}-{2}", p.inventory.items[14].item, p.inventory.items[14].count, p.inventory.items[14].meta);
			i15.Text = String.Format("{0}-{1}-{2}", p.inventory.items[15].item, p.inventory.items[15].count, p.inventory.items[15].meta);
			i16.Text = String.Format("{0}-{1}-{2}", p.inventory.items[16].item, p.inventory.items[16].count, p.inventory.items[16].meta);
			i17.Text = String.Format("{0}-{1}-{2}", p.inventory.items[17].item, p.inventory.items[17].count, p.inventory.items[17].meta);
			i18.Text = String.Format("{0}-{1}-{2}", p.inventory.items[18].item, p.inventory.items[18].count, p.inventory.items[18].meta);
			i19.Text = String.Format("{0}-{1}-{2}", p.inventory.items[19].item, p.inventory.items[19].count, p.inventory.items[19].meta);
			i20.Text = String.Format("{0}-{1}-{2}", p.inventory.items[20].item, p.inventory.items[20].count, p.inventory.items[20].meta);
			i21.Text = String.Format("{0}-{1}-{2}", p.inventory.items[21].item, p.inventory.items[21].count, p.inventory.items[21].meta);
			i22.Text = String.Format("{0}-{1}-{2}", p.inventory.items[22].item, p.inventory.items[22].count, p.inventory.items[22].meta);
			i23.Text = String.Format("{0}-{1}-{2}", p.inventory.items[23].item, p.inventory.items[23].count, p.inventory.items[23].meta);
			i24.Text = String.Format("{0}-{1}-{2}", p.inventory.items[24].item, p.inventory.items[24].count, p.inventory.items[24].meta);
			i25.Text = String.Format("{0}-{1}-{2}", p.inventory.items[25].item, p.inventory.items[25].count, p.inventory.items[25].meta);
			i26.Text = String.Format("{0}-{1}-{2}", p.inventory.items[26].item, p.inventory.items[26].count, p.inventory.items[26].meta);
			i27.Text = String.Format("{0}-{1}-{2}", p.inventory.items[27].item, p.inventory.items[27].count, p.inventory.items[27].meta);
			i28.Text = String.Format("{0}-{1}-{2}", p.inventory.items[28].item, p.inventory.items[28].count, p.inventory.items[28].meta);
			i29.Text = String.Format("{0}-{1}-{2}", p.inventory.items[29].item, p.inventory.items[29].count, p.inventory.items[29].meta);
			i30.Text = String.Format("{0}-{1}-{2}", p.inventory.items[30].item, p.inventory.items[30].count, p.inventory.items[30].meta);
			i31.Text = String.Format("{0}-{1}-{2}", p.inventory.items[31].item, p.inventory.items[31].count, p.inventory.items[31].meta);
			i32.Text = String.Format("{0}-{1}-{2}", p.inventory.items[32].item, p.inventory.items[32].count, p.inventory.items[32].meta);
			i33.Text = String.Format("{0}-{1}-{2}", p.inventory.items[33].item, p.inventory.items[33].count, p.inventory.items[33].meta);
			i34.Text = String.Format("{0}-{1}-{2}", p.inventory.items[34].item, p.inventory.items[34].count, p.inventory.items[34].meta);
			i35.Text = String.Format("{0}-{1}-{2}", p.inventory.items[35].item, p.inventory.items[35].count, p.inventory.items[35].meta);
			i36.Text = String.Format("{0}-{1}-{2}", p.inventory.items[36].item, p.inventory.items[36].count, p.inventory.items[36].meta);
			i37.Text = String.Format("{0}-{1}-{2}", p.inventory.items[37].item, p.inventory.items[37].count, p.inventory.items[37].meta);
			i38.Text = String.Format("{0}-{1}-{2}", p.inventory.items[38].item, p.inventory.items[38].count, p.inventory.items[38].meta);
			i39.Text = String.Format("{0}-{1}-{2}", p.inventory.items[39].item, p.inventory.items[39].count, p.inventory.items[39].meta);
			i40.Text = String.Format("{0}-{1}-{2}", p.inventory.items[40].item, p.inventory.items[40].count, p.inventory.items[40].meta);
			i41.Text = String.Format("{0}-{1}-{2}", p.inventory.items[41].item, p.inventory.items[41].count, p.inventory.items[41].meta);
			i42.Text = String.Format("{0}-{1}-{2}", p.inventory.items[42].item, p.inventory.items[42].count, p.inventory.items[42].meta);
			i43.Text = String.Format("{0}-{1}-{2}", p.inventory.items[43].item, p.inventory.items[43].count, p.inventory.items[43].meta);
			i44.Text = String.Format("{0}-{1}-{2}", p.inventory.items[44].item, p.inventory.items[44].count, p.inventory.items[44].meta);

			OnMouse.Text = String.Format("{0}-{1}-{2}", p.OnMouse.item, p.OnMouse.count, p.OnMouse.meta);

		}

		private void button1_Click(object sender, EventArgs e)
		{
			refreshz();
		}
	}
}
