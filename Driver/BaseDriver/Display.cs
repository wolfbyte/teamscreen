﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Driver.BaseDriver.Interface;
using Messages.Connection;
using Network.Manager;

namespace Driver.BaseDriver
{
	public abstract class Display : IDisplay
	{
		protected List<ClientListItem> ConnectedClients = new List<ClientListItem>();

		protected HostManager HostManager;

		protected System.Timers.Timer refreshThread;

		public Display()
		{
			refreshThread = new System.Timers.Timer(6000);
			refreshThread.Elapsed += RefreshThread_Elapsed;
		}

		public void SetHostManager(HostManager hostManager)
		{
			this.HostManager = hostManager;
		}

		private void RefreshThread_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{

			byte[] image = this.createScreenShot();

			ResponseScreenshotMessage rs = new ResponseScreenshotMessage();
			rs.Bounds = this.getBounds();
			rs.Fullscreen = true;
			rs.HostSystemId = HostManager.SystemId;
			rs.Image = image;

			foreach (var ID in ConnectedClients)
			{
				rs.ClientSystemId = ID.SystemId;
				HostManager.sendMessage(rs);
			}
		}

		protected abstract System.Drawing.Rectangle getBounds();

		protected abstract byte[] createScreenShot();

		public void StartScreenSharing(string clientSystemId)
		{

			if (!this.ConnectedClients.Contains(new ClientListItem(clientSystemId)))
			{
				this.ConnectedClients.Add(new ClientListItem(clientSystemId));
			}

			refreshThread.Start();
		}


	}
}
