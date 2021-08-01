using System;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using System.Linq;


namespace NicksMenuV2
{
	public class Loader
	{
		public static void Init()
		{
			Loader.Load = new GameObject();
			Loader.Load.AddComponent<Menu>();
			UnityEngine.Object.DontDestroyOnLoad(Loader.Load);
		}
		public static void Unload()
        {
			_Unload();
        }
		private static void _Unload()
        {
			GameObject.Destroy(Load);
        }


		private static GameObject Load;
	}


	public class Menu : MonoBehaviour
    {
		private void OnGUI()
		{
			UIHelper.Begin("Nicks Main Menu", 4f, 10f, 180f, (float)(this.ShowMainMenu ? 650 : 80), 4f, 20f, 2f);
			if (UIHelper.Button("Kill Menu"))
            {
				Loader.Unload();
            }
			if (UIHelper.Button("Show Menu: ", this.ShowMainMenu))
			{
				this.ShowMainMenu = !this.ShowMainMenu;
			}
		
			if (this.ShowMainMenu)
			{
				UIHelper.Label("[Character Modifications]");
				UIHelper.Label($"[Sprint Speed:{_Body.sprintingSpeedMultiplier}]");
				if (UIHelper.Button("Increase")) { Increase_Sprint_Speed = true; }
				if (UIHelper.Button("Decrease")) { Decrease_Sprint_Speed = true; }
				UIHelper.Label($"[Player Health:{_Body.maxHealth}]");
				if (UIHelper.Button("Increase")) { Increase_Player_Health = true; }
				if (UIHelper.Button("Decrease")) { Decrease_Player_Health = true; }
				UIHelper.Label($"[Player Attack:{_Body.damage}]");
				if (UIHelper.Button("Increase")) { Increase_Player_Attack = true; }
				if (UIHelper.Button("Decrease")) { Decrease_Player_Attack = true; }
				UIHelper.Label($"[Coins,Money,Experience]");
				if (UIHelper.Button("Give Lunar Coins")) { Give_Coins = true; }
				if (UIHelper.Button("Give Money")) { Give_Money = true; }
				if (UIHelper.Button("Give Experience")) { Give_Experience = true; }
				UIHelper.Label("[Game Modifications]");
				if (UIHelper.Button("Spawn Portals")) { Spawn_Portals = true; }
				if (UIHelper.Button("Charge Portal")) { Charge_Portal = true; }
				if (UIHelper.Button("Skip Stage")) { Skip_Stage = true; }
				UIHelper.Label("[ESP]");
				if (UIHelper.Button("Show Portal: ",Show_Portal)){ Show_Portal = !Show_Portal; }
				if (UIHelper.Button("Show Interactables: ",Show_Interactables)){ Show_Interactables = !Show_Interactables; }

				UIHelper.Label("Extras");
				if(UIHelper.Button("Aimbot: ", Mode_Aimbot)) { Mode_Aimbot = !Mode_Aimbot; }
				if (UIHelper.Button("Sortve God Mode: ", Mode_God)) { Mode_God = !Mode_God; }
				if (UIHelper.Button("Unlock Achievements")) { Unlock_All = true; }


			}
			ModHandler();

		}

	


		private void ModHandler()
        {
			//Working
            if (Increase_Sprint_Speed)
            {
				Increase_Sprint_Speed = !Increase_Sprint_Speed;
				_Body.sprintingSpeedMultiplier += 1f;
            }
			if (Decrease_Sprint_Speed)
            {
				Decrease_Sprint_Speed = !Decrease_Sprint_Speed;
				if (_Body.sprintingSpeedMultiplier -1f < 0)
                {
					_Body.sprintingSpeedMultiplier -= 1f;

				}
            }
			//Working
            if (Increase_Player_Health)
            {
				Increase_Player_Health = !Increase_Player_Health;
				_Body.baseMaxHealth += 100f;
			}
			if (Decrease_Player_Health)
            {
				Decrease_Player_Health = !Decrease_Player_Health;
				_Body.baseMaxHealth -= 100f;
            }
			//Working
			if (Increase_Player_Attack)
            {
				Increase_Player_Attack = !Increase_Player_Attack;
				_Body.baseDamage += 10f;
            }
			if (Decrease_Player_Attack)
            {
				Decrease_Player_Attack = !Decrease_Player_Attack;
				_Body.baseDamage -= 10f;
			}
			//Working
			if (Spawn_Portals)
            {
				Spawn_Portals = !Spawn_Portals;
				TeleporterInteraction.instance.shouldAttemptToSpawnGoldshoresPortal = true;
				TeleporterInteraction.instance.shouldAttemptToSpawnShopPortal = true;
				TeleporterInteraction.instance.shouldAttemptToSpawnMSPortal = true;

			}
			if (Charge_Portal)
            {
				Charge_Portal = !Charge_Portal;
				typeof(HoldoutZoneController).GetProperty("charge").SetValue(TeleporterInteraction.instance.holdoutZoneController, 1f);
			}
			if (Skip_Stage)
            {
				Skip_Stage = !Skip_Stage;
				Run.instance.AdvanceStage(Run.instance.nextStageScene);
			}
			//Working
			if (Give_Coins)
            {
				Give_Coins = !Give_Coins;
				_NetworkUser.BroadcastMessage("Suck my nuts");
				_NetworkUser.NetworknetLunarCoins = HGMath.UintSafeAdd(this._NetworkUser.NetworknetLunarCoins, 1000u);
				_NetworkUser.CallRpcAwardLunarCoins(1000u);
				

            }
			if (Give_Money)
            {
				Give_Money = !Give_Money;
				_TeamManager.GiveTeamMoney(TeamIndex.Player, 500u);
				_Master.GiveMoney(500u);
            }
			if (Give_Experience)
            {
				Give_Experience = !Give_Experience;
				_Master.GiveExperience(5000u);
				_TeamManager.GiveTeamExperience(TeamIndex.Player, 5000u);
			}			
			//Working
			if (Show_Portal)
            {
				RenderTeleporter();
            }
            if (Show_Interactables)
            {
				RenderInteractables();
            }

			//Extra
			if (Mode_Aimbot)
            {
				Aimbot();
            }
			if (Mode_God)
            {
				Mode_God = !Mode_God;
				_Body.baseMaxHealth = float.MaxValue;
            }

			if (Unlock_All)
            {
				Unlock_All = !Unlock_All;
				UnlockAll();
            }
		
           
        }


		public void RenderTeleporter()
		{
			Vector3 W2S = Camera.main.WorldToScreenPoint(this._Teleporter.transform.position);
			W2S.y = (float)Screen.height - W2S.y;
			if (W2S.z > 0f)
			{
				GUI.color = Color.green;
				GUI.Label(new Rect(W2S.x, W2S.y, 200f, 20f), "Teleporter");
			}
		}
		public void RenderInteractables()
		{
			foreach (PurchaseInteraction purchaseInteraction in PurchaseInteraction.FindObjectsOfType(typeof(PurchaseInteraction)))
			{
				if (purchaseInteraction.available)
				{
					Vector3 vector = Camera.main.WorldToScreenPoint(purchaseInteraction.transform.position);
					if ((double)vector.z > 0.01)
					{
						GUI.color = Color.cyan;
						string displayName = purchaseInteraction.GetDisplayName();
						GUI.Label(new Rect(vector.x, (float)Screen.height - vector.y, 100f, 50f), displayName);
					}
				}
			}
		}
		public void Aimbot()
		{
			PlayerCharacterMasterController cachedMasterController = LocalUserManager.GetFirstLocalUser().cachedMasterController;
			if (cachedMasterController)
			{
				CharacterBody body = cachedMasterController.master.GetBody();
				if (body)
				{
					InputBankTest component = body.GetComponent<InputBankTest>();
					Ray ray = new Ray(component.aimOrigin, component.aimDirection);
					BullseyeSearch bullseyeSearch = new BullseyeSearch();
					TeamComponent component2 = body.GetComponent<TeamComponent>();
					bullseyeSearch.teamMaskFilter = TeamMask.all;
					bullseyeSearch.teamMaskFilter.RemoveTeam(component2.teamIndex);
					bullseyeSearch.filterByLoS = true;
					bullseyeSearch.searchOrigin = ray.origin;
					bullseyeSearch.searchDirection = ray.direction;
					bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
					bullseyeSearch.maxDistanceFilter = float.MaxValue;
					bullseyeSearch.maxAngleFilter = 179.9f;
					bullseyeSearch.RefreshCandidates();
					HurtBox hurtBox = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
					if (hurtBox)
					{
						Vector3 aimDirection = hurtBox.transform.position - ray.origin;
						component.aimDirection = aimDirection;
					}
				}
			}
		}
		public void UnlockAll()
        {
			UserProfile userProfile = LocalUserManager.GetFirstLocalUser().userProfile;

			foreach (ItemIndex item in ItemCatalog.allItems)
            {
				userProfile.DiscoverPickup(PickupCatalog.FindPickupIndex(item));
            }
			foreach (EquipmentIndex equipmentIndex in EquipmentCatalog.allEquipment)
			{
				userProfile.DiscoverPickup(PickupCatalog.FindPickupIndex(equipmentIndex));
			}

			UserAchievementManager UAM = AchievementManager.GetUserAchievementManager(RoR2.LocalUserManager.GetFirstLocalUser());
			foreach (AchievementDef AD in AchievementManager.allAchievementDefs)
            {
				UAM.GrantAchievement(AD);
            }






		}


		private void Start()
        {
			_Body = FindObjectOfType<CharacterBody>();
			_Master = FindObjectOfType<CharacterMaster>();
			_NetworkUser = FindObjectOfType<NetworkUser>();
			_TeamManager = FindObjectOfType<TeamManager>();
			_Teleporter = FindObjectOfType<TeleporterInteraction>();
		}

		private void Update()
        {
			ModHandler();
			_Teleporter = FindObjectOfType<TeleporterInteraction>();
			if (_Body == null || _Body.Equals(FindObjectOfType<CharacterBody>()))
			{
				_Body = FindObjectOfType<CharacterBody>();

			}
			if (_Master == null || _Master.Equals(FindObjectOfType<CharacterBody>()))
			{
				_Master = FindObjectOfType<CharacterMaster>();

			}
			if (_NetworkUser == null || _NetworkUser.Equals(FindObjectOfType<NetworkUser>()))
			{
				_NetworkUser = FindObjectOfType<NetworkUser>();

			}
			if (_TeamManager == null || _TeamManager.Equals(FindObjectOfType<TeamManager>()))
			{
				_TeamManager = FindObjectOfType<TeamManager>();

			}
		}

		




		//MENUS
		private bool ShowMainMenu = true;

		//Sprint Speed
		private bool Increase_Sprint_Speed = false;
		private bool Decrease_Sprint_Speed = false;
		//Health Amount
		private bool Increase_Player_Health = false;
		private bool Decrease_Player_Health = false;
		//Attack Damage
		private bool Increase_Player_Attack = false;
		private bool Decrease_Player_Attack = false;

		//Portal Modifications
		private bool Spawn_Portals = false;
		private bool Charge_Portal = false;
		private bool Skip_Stage = false;

		//Coins,Money,Experience
		private bool Give_Coins = false;
		private bool Give_Money = false;
		private bool Give_Experience = false;

		//ESP
		private bool Show_Portal = false;
		private bool Show_Interactables = false;

		//Extras 
		private bool Mode_Aimbot = false;
		private bool Mode_God = false;
		private bool Unlock_All = false;


		//ROR Variables
		RoR2.CharacterBody _Body;
		RoR2.CharacterMaster _Master;
        RoR2.NetworkUser _NetworkUser;
		RoR2.TeamManager _TeamManager;
		RoR2.TeleporterInteraction _Teleporter;



    }


    public static class UIHelper
	{
		// Token: 0x0600000D RID: 13 RVA: 0x00002620 File Offset: 0x00000820
		public static void Begin(string text, float _x, float _y, float _width, float _height, float _margin, float _controlHeight, float _controlDist)
		{
			UIHelper.x = _x;
			UIHelper.y = _y;
			UIHelper.width = _width;
			UIHelper.height = _height;
			UIHelper.margin = _margin;
			UIHelper.controlHeight = _controlHeight;
			UIHelper.controlDist = _controlDist;
			UIHelper.nextControlY = _y + 20f;
			GUI.Box(new Rect(UIHelper.x, UIHelper.y, UIHelper.width, UIHelper.height), text);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002688 File Offset: 0x00000888
		private static Rect NextControlRect()
		{
			Rect result = new Rect(UIHelper.x + UIHelper.margin, UIHelper.nextControlY, UIHelper.width - UIHelper.margin * 2f, UIHelper.controlHeight);
			UIHelper.nextControlY += UIHelper.controlHeight + UIHelper.controlDist;
			return result;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000026D6 File Offset: 0x000008D6
		public static string MakeEnable(string text, bool state)
		{
			return string.Format("{0}{1}", text, state ? "ON" : "OFF");
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000026F2 File Offset: 0x000008F2
		public static bool Button(string text, bool state)
		{
			return UIHelper.Button(UIHelper.MakeEnable(text, state));
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002700 File Offset: 0x00000900
		public static bool Button(string text, params string[] args)
		{
			return GUI.Button(UIHelper.NextControlRect(), string.Format(text, args));
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002713 File Offset: 0x00000913
		public static bool Button(string text)
		{
			return GUI.Button(UIHelper.NextControlRect(), text);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002720 File Offset: 0x00000920
		public static void Label(string text, float value, int decimals = 2)
		{
			UIHelper.Label(string.Format("{0}{1}", text, Math.Round((double)value, 2).ToString()));
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000274D File Offset: 0x0000094D
		public static void Label(string text)
		{
			GUI.Label(UIHelper.NextControlRect(), text);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000275A File Offset: 0x0000095A
		public static float Slider(float val, float min, float max)
		{
			return GUI.HorizontalSlider(UIHelper.NextControlRect(), val, min, max);
		}

		// Token: 0x0400000C RID: 12
		private static float x;

		// Token: 0x0400000D RID: 13
		private static float y;

		// Token: 0x0400000E RID: 14
		private static float width;

		// Token: 0x0400000F RID: 15
		private static float height;

		// Token: 0x04000010 RID: 16
		private static float margin;

		// Token: 0x04000011 RID: 17
		private static float controlHeight;

		// Token: 0x04000012 RID: 18
		private static float controlDist;

		// Token: 0x04000013 RID: 19
		private static float nextControlY;

	}


}