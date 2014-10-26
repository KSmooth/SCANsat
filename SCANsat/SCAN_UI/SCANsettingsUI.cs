﻿#region license
/* 
 *  [Scientific Committee on Advanced Navigation]
 * 			S.C.A.N. Satellite
 *
 * SCANsat - Settings menu window object
 * 
 * Copyright (c)2013 damny;
 * Copyright (c)2014 David Grandy <david.grandy@gmail.com>;
 * Copyright (c)2014 technogeeky <technogeeky@gmail.com>;
 * Copyright (c)2014 (Your Name Here) <your email here>; see LICENSE.txt for licensing details.
 *
 */
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using SCANsat.Platform;
using SCANsat.Platform.Palettes;
using UnityEngine;

using palette = SCANsat.SCAN_UI.SCANpalette;

namespace SCANsat.SCAN_UI
{
	class SCANsettingsUI: SCAN_MBW
	{
		/* UI: a list of glyphs that are used for something */
		private string[] exmarks = { "✗", "✘", "×", "✖", "x", "X", "∇", "☉", "★", "*", "•", "º", "+" };

		/* UI: time warp names and settings */
		private string[] twnames = { "Off", "Low", "Medium", "High" };
		private int[] twvals = { 1, 6, 9, 15 };
		private bool warningBoxOne, warningBoxAll, spaceCenterLock, trackingStationLock;
		private Rect warningRect;
		private const string lockID = "settingLockID";

		internal static Rect defaultRect = new Rect(Screen.width - (Screen.width / 2) - 180, 100, 360, 300);

		protected override void Awake()
		{
			WindowCaption = "S.C.A.N. Settings";
			WindowRect = defaultRect;
			WindowStyle = SCANskins.SCAN_window;
			WindowOptions = new GUILayoutOption[2] { GUILayout.Width(360), GUILayout.Height(300) };
			Visible = false;
			DragEnabled = true;
			ClampToScreenOffset = new RectOffset(-280, -280, -600, -600);

			SCAN_SkinsLibrary.SetCurrent("SCAN_Unity");

			InputLockManager.RemoveControlLock(lockID);
		}

		internal override void OnDestroy()
		{
			InputLockManager.RemoveControlLock(lockID);
		}

		internal override void Start()
		{
		}

		protected override void DrawWindowPre(int id)
		{
			//Lock space center click through
			if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
			{
				Vector2 mousePos = Input.mousePosition;
				mousePos.y = Screen.height - mousePos.y;
				if (WindowRect.Contains(mousePos) && !spaceCenterLock)
				{
					InputLockManager.SetControlLock(ControlTypes.CAMERACONTROLS | ControlTypes.KSC_FACILITIES | ControlTypes.KSC_UI, lockID);
					spaceCenterLock = true;
				}
				else if (!WindowRect.Contains(mousePos) && spaceCenterLock)
				{
					InputLockManager.RemoveControlLock(lockID);
					spaceCenterLock = false;
				}
			}

			//Lock tracking scene click through
			if (HighLogic.LoadedScene == GameScenes.TRACKSTATION)
			{
				Vector2 mousePos = Input.mousePosition;
				mousePos.y = Screen.height - mousePos.y;
				if (WindowRect.Contains(mousePos) && !trackingStationLock)
				{
					InputLockManager.SetControlLock(ControlTypes.CAMERACONTROLS | ControlTypes.TRACKINGSTATION_ALL, lockID);
					trackingStationLock = true;
				}
				else if (!WindowRect.Contains(mousePos) && trackingStationLock)
				{
					InputLockManager.RemoveControlLock(lockID);
					trackingStationLock = false;
				}
			}
		}

		protected override void DrawWindow(int id)
		{
			versionLabel(id);
			closeBox(id);

			growS();
				gui_settings_xmarks(id); 				/* X marker selection */
				gui_settings_resources(id);				/* resource details sub-window */
				gui_settings_toggle_body_scanning(id);	/* background and body scanning toggles */
				gui_settings_rebuild_kethane(id);		/* rebuild Kethane database with SCANsat info */
				gui_settings_timewarp(id);				/* time warp resolution settings */
				gui_settings_numbers(id);				/* sensor/scanning		statistics */
				gui_settings_data_resets(id);			/* reset data and/or reset resources */
				gui_settings_window_resets(id);			/* reset windows and positions */
				# if DEBUG
					gui_settings_window_mapFill(id);	/* debug option to fill in maps */
				#endif
			stopS();

			warningBox(id);
		}

		protected override void DrawWindowPost(int id)
		{
			if ((warningBoxOne || warningBoxAll) && Event.current.type == EventType.mouseDown && !warningRect.Contains(Event.current.mousePosition))
			{
				warningBoxOne = false;
				warningBoxAll = false;
			}
		}

		//Draw the version label in the upper left corner
		private void versionLabel(int id)
		{
			Rect r = new Rect(6, 0, 40, 18);
			GUI.Label(r, SCANversions.SCANsatVersion, SCANskins.SCAN_whiteReadoutLabel);
		}

		//Draw the close button in the upper right corner
		private void closeBox(int id)
		{
			Rect r = new Rect(WindowRect.width - 20, 0, 18, 18);
			if (GUI.Button(r, SCANcontroller.controller.closeBox, SCANskins.SCAN_closeButton))
			{
				InputLockManager.RemoveControlLock(lockID);
				trackingStationLock = false;
				spaceCenterLock = false;
				Visible = false;
			}
		}

		//Choose anomaly marker icon
		private void gui_settings_xmarks(int id)
		{
			fillS(8);
			GUILayout.Label("Anomaly Marker", SCANskins.SCAN_headline);
			growE();
			for (int i = 0; i < exmarks.Length; ++i)
			{
				if (SCANcontroller.controller.anomalyMarker == exmarks[i])
				{
					if (GUILayout.Button(exmarks[i], SCANskins.SCAN_closeButton))
						SCANcontroller.controller.anomalyMarker = exmarks[i];
				}
				else
				{
					if (GUILayout.Button(exmarks[i], SCANskins.SCAN_buttonBorderless))
						SCANcontroller.controller.anomalyMarker = exmarks[i];
				}
			}
			stopE();
			fillS(16);
		}

		//Control resource options - *Will be moved into big map*
		private void gui_settings_resources(int id)
		{
			GUILayout.Label("Resources Overlay", SCANskins.SCAN_headline);
			if (SCANcontroller.controller.ResourcesList.Count > 0)
			{
				if (SCANcontroller.controller.globalOverlay != GUILayout.Toggle(SCANcontroller.controller.globalOverlay, "Activate Resource Overlay"))
				{ //global toggle for resource overlay
					SCANcontroller.controller.globalOverlay = !SCANcontroller.controller.globalOverlay;
					//if (bigmap != null) bigmap.resetMap();
				}
			}

			growE();
			if (GUILayout.Button("Kethane Resources")) //select from two resource types, populates the list below
			{
				SCANcontroller.controller.resourceOverlayType = 1;
				SCANcontroller.controller.Resources(FlightGlobals.currentMainBody);
				if (SCANcontroller.controller.ResourcesList.Count > 0)
					SCANcontroller.controller.globalOverlay = true;
				//if (bigmap != null) bigmap.resetMap();
			}

			if (GUILayout.Button("ORSX Resources"))
			{
				SCANcontroller.controller.resourceOverlayType = 0;
				SCANcontroller.controller.Resources(FlightGlobals.currentMainBody);
				if (SCANcontroller.controller.ResourcesList.Count > 0)
					SCANcontroller.controller.globalOverlay = true;
				//if (bigmap != null) bigmap.resetMap();
			}
			stopE();
			if (SCANcontroller.controller.ResourcesList.Count == 0)
			{
				fillS(5);
				GUILayout.Label("No Resources Found", SCANskins.SCAN_headline);
			}
			growE();
			SCANcontroller.controller.gridSelection = GUILayout.SelectionGrid(SCANcontroller.controller.gridSelection, SCANcontroller.controller.ResourcesList.Select(a => a.Name).ToArray(), 4); //select resource to display
			stopE();
			fillS(16);
		}

		//Control background scanning options
		private void gui_settings_toggle_body_scanning(int id)
		{

			GUILayout.Label("Background Scanning", SCANskins.SCAN_headline);
			// scan background
			SCANcontroller.controller.scan_background = GUILayout.Toggle(SCANcontroller.controller.scan_background, "Scan all active celestials");
			// scanning for individual SoIs
			growE();
			int count = 0;
			foreach (var data in SCANcontroller.Body_Data)
			{
				if (count == 0) growS();
					data.Value.Disabled = !GUILayout.Toggle(!data.Value.Disabled, string.Format("{0} ({1:N1}%)", data.Key, data.Value.getCoveragePercentage(SCANdata.SCANtype.Nothing)));
				switch (count)
				{
					case 5: stopS(); count = 0; break;
					default: ++count; break;
				}
			}
			if (count != 0)
				stopS(); ;
			stopE();
		}

		//Update the Kethane database to reset the map grid
		private void gui_settings_rebuild_kethane(int id)
		{
			if (SCANcontroller.controller.resourceOverlayType == 1 && SCANcontroller.controller.globalOverlay)
			{ //Rebuild the Kethane database
				if (GUILayout.Button("Rebuild Kethane Grid Database"))
					SCANcontroller.controller.KethaneRebuild = !SCANcontroller.controller.KethaneRebuild;
			}
			fillS(16);
		}

		//Control scanning resolution
		private void gui_settings_timewarp(int id)
		{
			GUILayout.Label("Time Warp Resolution", SCANskins.SCAN_headline);
			growE();

			for (int i = 0; i < twnames.Length; ++i)
			{
				if (SCANcontroller.controller.timeWarpResolution == twvals[i])
				{
					if (GUILayout.Button(twnames[i], SCANskins.SCAN_buttonActive))
						SCANcontroller.controller.timeWarpResolution = twvals[i];
				}
				else
				{
					if (GUILayout.Button(twnames[i], SCANskins.SCAN_buttonFixed))
						SCANcontroller.controller.timeWarpResolution = twvals[i];
				}
			}
			stopE();
			fillS(8);
		}

		//Display the total number of SCANsat sensors and scanning passes
		private void gui_settings_numbers(int id)
		{
			string s = 	"Vessels: " + SCANcontroller.controller.ActiveVessels.ToString() +
						" Sensors: " + SCANcontroller.controller.ActiveSensors +
						" Passes: " + SCANcontroller.controller.ActualPasses.ToString();
			GUILayout.Label(s, SCANskins.SCAN_whiteReadoutLabel);
			fillS(16);
		}

		//Reset databases - *Needs confirmation box*
		private void gui_settings_data_resets(int id)
		{
			CelestialBody thisBody = FlightGlobals.currentMainBody;
			GUILayout.Label("Data Management", SCANskins.SCAN_headline);
			growE();
			if (warningBoxOne || warningBoxAll)
			{
				GUILayout.Label("Reset map of " + thisBody.theName, SCANskins.SCAN_button);
				GUILayout.Label("Reset <b>all</b> data", SCANskins.SCAN_button);
			}
			else
			{
				if (GUILayout.Button("Reset map of " + thisBody.theName))
				{
					warningBoxOne = true;
				}
				if (GUILayout.Button("Reset <b>all</b> data"))
				{
					warningBoxAll = true;
				}
			}
			stopE();
			fillS(8);
		}

		//Resets all window positions
		private void gui_settings_window_resets(int id)
		{
			if (GUILayout.Button("Reset window positions", SCANskins.SCAN_buttonFixed))
			{
				if (HighLogic.LoadedSceneIsFlight)
				{
					SCANuiUtil.resetMainMapPos();
					SCANuiUtil.resetBigMapPos();
					SCANuiUtil.resetInstUIPos();
					SCANuiUtil.resetSettingsUIPos();
				}
				else
				{
					SCANuiUtil.resetKSCMapPos();
				}
			}
			fillS(8);
		}

		//Debugging option to fill in SCAN maps
		private void gui_settings_window_mapFill(int id)
		{
			growE();
			CelestialBody thisBody = FlightGlobals.currentMainBody;
			if (GUILayout.Button("Fill SCAN map of " + thisBody.theName, SCANskins.SCAN_buttonFixed))
			{
				SCANdata data = SCANUtil.getData(thisBody);
				if (data == null)
				{
					data = new SCANdata(thisBody);
					SCANcontroller.controller.addToBodyData(thisBody, data);
				}
				data.fillMap();
			}
			if (GUILayout.Button("Fill SCAN map for all planets", SCANskins.SCAN_buttonFixed))
			{
				foreach (CelestialBody b in FlightGlobals.Bodies)
				{
					SCANdata data = SCANUtil.getData(b);
					if (data == null)
					{
						data = new SCANdata(b);
						SCANcontroller.controller.addToBodyData(b, data);
					}
					data.fillMap();
				}
			}
			stopE();
			fillS(8);
		}

		//Confirmation boxes for map resets
		private void warningBox(int id)
		{
			if (warningBoxOne)
			{
				CelestialBody thisBody = FlightGlobals.currentMainBody;
				warningRect = new Rect(WindowRect.width - (WindowRect.width / 2)- 150, WindowRect.height - 190, 300, 90);
				GUI.Box(warningRect, "", SCANskins.SCAN_dropDownBox);
				Rect r = new Rect(warningRect.x + 10, warningRect.y + 5, 280, 40);
				GUI.Label(r, "Erase all data for " + thisBody.theName + "?", SCANskins.SCAN_headlineSmall);
				r.x += 90;
				r.y += 45;
				r.width = 80;
				r.height = 30;
				if (GUI.Button(r, "Confirm", SCANskins.SCAN_buttonWarning))
				{
					warningBoxOne = false;
					SCANdata data = SCANUtil.getData(thisBody);
					if (data != null)
						data.reset();
				}
			}
			else if (warningBoxAll)
			{
				warningRect = new Rect(WindowRect.width - (WindowRect.width / 2) - 120, WindowRect.height - 190, 240, 90);
				GUI.Box(warningRect, "", SCANskins.SCAN_dropDownBox);
				Rect r = new Rect(warningRect.x + 10, warningRect.y + 5, 220, 40);
				GUI.Label(r, "Erase <b>all</b> data ?", SCANskins.SCAN_headlineSmall);
				r.x += 70;
				r.y += 45;
				r.width = 80;
				r.height = 30;
				if (GUI.Button(r, "Confirm", SCANskins.SCAN_buttonWarning))
				{
					warningBoxAll = false;
					foreach (SCANdata data in SCANcontroller.Body_Data.Values)
					{
						data.reset();
					}
				}
			}
		}

	}
}