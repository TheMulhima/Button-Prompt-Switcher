using MonoMod.RuntimeDetour;
using UnityEngine.UI;
using Satchel.Reflected;

namespace Button_Prompt_Switcher;

public class Button_Prompt_Switcher : Mod, IGlobalSettings<GlobalSettings>, ICustomMenuMod
{
    internal static Button_Prompt_Switcher Instance;

    public static GlobalSettings settings { get; set; } = new GlobalSettings();
    public void OnLoadGlobal(GlobalSettings s) => settings = s;
    public GlobalSettings OnSaveGlobal() => settings;

    public new string GetName() => "Button Prompt Switcher";
    public override string GetVersion() => AssemblyUtils.GetAssemblyVersionHash();

    public override void Initialize()
    {
        Instance ??= this;
        
        On.ControllerDetect.LookForActiveController += LookForActiveController;

        On.HeroController.CanCast += CanCast;
        
        _ = new ILHook(
	        typeof(InputManager)
	        .GetMethod(nameof(InputManager.AttachDevice)),
	        AttachDeviceIL);
        
        //there are 3 overloads and luckily the correct overload is edited by mapi so i can il hook the orig_ version
	    _ = new ILHook(
	        typeof(UIButtonSkins)
		        .GetMethod("orig_GetButtonSkinFor", BindingFlags.Instance | BindingFlags.NonPublic),
	        GetButtonSkinForIL);
    }

    private bool CanCast(On.HeroController.orig_CanCast orig, HeroController self)
    {
	    //get value of original conditions
	    var canCast = orig(self);

	    if (settings.NoCast)
	    {
		    canCast = canCast && !HeroControllerR.inputHandler.inputActions.cast.WasReleased;
	    }

	    return canCast;

    }

    private void LookForActiveController(On.ControllerDetect.orig_LookForActiveController orig, ControllerDetect self)
    {
	    if (InputHandler.Instance.gamepadState == GamepadState.DETACHED)
	    {
		    ReflectionHelper.CallMethod(self,"HideButtonLabels");
		    ReflectionHelper.GetField<ControllerDetect, Image>(self, "controllerImage").sprite = self.controllerImages[0].sprite;
		    UIManager.instance.ShowCanvasGroup(self.controllerPrompt);
		    self.remapButton.gameObject.SetActive(false);
		    return;
	    }
	    if (InputHandler.Instance.activeGamepadType != GamepadType.NONE)
	    {
		    UIManager.instance.HideCanvasGroup(self.controllerPrompt);
		    self.remapButton.gameObject.SetActive(true);
		    
		    //added code
		    if (settings.ButtonSkinTypeIsUsed)
		    {
			    switch (settings.ButtonSkinType)
			    {
				    case 0:
					    ReflectionHelper.CallMethod(self, "ShowController", GamepadType.SWITCH_JOYCON_DUAL);
					    return;
				    case 1:
				    case 2:
					    ReflectionHelper.CallMethod(self, "ShowController", GamepadType.PS4);
					    return;
				    case 3:
					    ReflectionHelper.CallMethod(self, "ShowController", GamepadType.PS3_WIN);
					    return;
				    case 4:
					    ReflectionHelper.CallMethod(self, "ShowController", GamepadType.XBOX_ONE);
					    return;
				    case 5:
					    ReflectionHelper.CallMethod(self, "ShowController", GamepadType.XBOX_360);
					    return;
				    default:
					    ReflectionHelper.CallMethod(self, "ShowController", InputHandler.Instance.activeGamepadType);
					    return;
			    }
		    }
		    else
		    {
			    ReflectionHelper.CallMethod(self, "ShowController", InputHandler.Instance.activeGamepadType);
		    }
	    }
    }

    private void GetButtonSkinForIL(ILContext il)
    {
	    var cursor = new ILCursor(il).Goto(0);

	    //go just before the last line
	    cursor.GotoNext(MoveType.Before, i => i.MatchRet());
        //get the current instance of the class for geting values
        cursor.Emit(OpCodes.Ldarg_0);
        
        //get value of first argument (input control type)
        cursor.Emit(OpCodes.Ldarg_1);
        
        //the ButtonSkin is already on the stack before the ret

        //take in the button skin (return value), the class instance and first argument of the function
        //and use that to add this piece of code just before the function returns
        cursor.EmitDelegate<Func<ButtonSkin,UIButtonSkins,InputControlType, ButtonSkin>>(
	        (buttonSkin, _UIButtonSkins, inputControlType) =>
	        {
		        if (settings.ButtonSkinTypeIsUsed)
		        {
			        int buttonSkinType = settings.ButtonSkinType;
			        int num = (buttonSkinType != 0) ? ((buttonSkinType > 3) ? 2 : 1) : 0;
			        switch (inputControlType)
			        {
				        case InputControlType.DPadUp:
				        {
					        Sprite[] array = new Sprite[]
					        {
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "switchHidDPadUp"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "dpadUp"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "dpadUp"),
					        };
					        buttonSkin.sprite = array[num];
					        break;
				        }
				        case InputControlType.DPadDown:
				        {
					        Sprite[] array2 = new Sprite[]
					        {
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "switchHidDPadDown"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "dpadDown"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "dpadDown"),
					        };
					        buttonSkin.sprite = array2[num];
					        break;
				        }
				        case InputControlType.DPadLeft:
				        {
					        Sprite[] array3 = new Sprite[]
					        {
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "switchHidDPadLeft"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "dpadLeft"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "dpadLeft"),
					        };
					        buttonSkin.sprite = array3[num];
					        break;
				        }
				        case InputControlType.DPadRight:
				        {
					        Sprite[] array4 = new Sprite[]
					        {
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "switchHidDPadRight"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "dpadRight"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "dpadRight"),
					        };
					        buttonSkin.sprite = array4[num];
					        break;
				        }
				        case InputControlType.LeftTrigger:
				        {
					        Sprite[] array5 = new Sprite[]
					        {
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins,
							        "switchHidLeftTrigger"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "ps4lt"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "lt"),
					        };
					        buttonSkin.sprite = array5[num];
					        break;
				        }
				        case InputControlType.RightTrigger:
				        {
					        Sprite[] array6 = new Sprite[]
					        {
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins,
							        "switchHidRightTrigger"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "ps4rt"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "rt"),
					        };
					        buttonSkin.sprite = array6[num];
					        break;
				        }
				        case InputControlType.LeftBumper:
				        {
					        Sprite[] array7 = new Sprite[]
					        {
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "switchHidLeftBumper"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "ps4lb"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "lb"),
					        };
					        buttonSkin.sprite = array7[num];
					        break;
				        }
				        case InputControlType.RightBumper:
				        {
					        Sprite[] array8 = new Sprite[]
					        {
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins,
							        "switchHidRightBumper"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "ps4rb"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "rb"),
					        };
					        buttonSkin.sprite = array8[num];
					        break;
				        }
				        case InputControlType.Action1:
				        {
					        Sprite[] array9 = new Sprite[]
					        {
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "switchHidB"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "ps4x"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "a"),
					        };
					        buttonSkin.sprite = array9[num];
					        break;
				        }
				        case InputControlType.Action2:
				        {
					        Sprite[] array10 = new Sprite[]
					        {
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "switchHidA"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "ps4circle"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "b"),
					        };
					        buttonSkin.sprite = array10[num];
					        break;
				        }
				        case InputControlType.Action3:
				        {
					        Sprite[] array11 = new Sprite[]
					        {
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "switchHidY"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "ps4square"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "x"),
					        };
					        buttonSkin.sprite = array11[num];
					        break;
				        }
				        case InputControlType.Action4:
				        {
					        Sprite[] array12 = new Sprite[]
					        {
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "switchHidX"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "ps4triangle"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "y"),
					        };
					        buttonSkin.sprite = array12[num];
					        break;
				        }
				        default:
				        {
					        switch (inputControlType)
					        {
						        case InputControlType.Back:
						        case InputControlType.Select:
						        case InputControlType.Share:
						        case InputControlType.View:
							        break;
						        case InputControlType.Start:
						        case InputControlType.Options:
						        case InputControlType.Menu:
						        {
							        Sprite[] array13 = new Sprite[]
							        {
								        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins,
									        "switchHidPlus"),
								        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "options"),
								        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "options"),
								        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "start"),
								        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "menu"),
								        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "start"),
							        };
							        buttonSkin.sprite = array13[buttonSkinType];
							        return buttonSkin;
						        }
						        case InputControlType.System:
						        case InputControlType.Pause:
						        case InputControlType.Home:
							        return buttonSkin;
						        default:
							        if (inputControlType != InputControlType.TouchPadButton)
							        {
								        return buttonSkin;
							        }

							        break;
					        }

					        Sprite[] array14 = new Sprite[]
					        {
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "switchHidMinus"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "touchpadButton"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "share"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "select"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "view"),
						        ReflectionHelper.GetField<UIButtonSkins, Sprite>(_UIButtonSkins, "select"),
					        };
					        buttonSkin.sprite = array14[buttonSkinType];
					        break;
				        }
			        }
		        }

		        return buttonSkin;
	        });

    }


    private void AttachDeviceIL(ILContext il)
    {
	    var cursor = new ILCursor(il).Goto(0);
        //replace the condition in
        /*if (!inputDevice.IsSupportedOnThisPlatform)
        {
            return;
        }*/
        cursor.GotoNext(MoveType.After, i => i.MatchCallvirt(out _));
        
        //i want the value of first arg (the input device) so i emit it and take it into the func (is ldarg0 cuz its static)
        cursor.Emit(OpCodes.Ldarg_0);
        
        //get the original return value and the value of first argument and replace the bool with my conditions
        cursor.EmitDelegate<Func<bool, InputDevice, bool>>((oldvalue, inputDevice) =>
        {
            if (settings.EmulatedXboxOnly && !inputDevice.Name.Equals("XInput Controller") &&
                !inputDevice.Name.Equals("Xbox 360 Controller") && !inputDevice.Name.Equals("XBox 360 Controller") &&
                !inputDevice.Name.Equals("Microsoft Xbox 360 Controller"))
            {
                //we are before a brtrue, so we need to return false to not branch and reach the return statement
                return false;
            }

            if (inputDevice.IsUnknown)
            {
                return false;
            }

            //dont edit the code
            return oldvalue;
        });
    }

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? _) =>
        ModMenu.CreateMenuScreen(modListMenu);


    public bool ToggleButtonInsideMenu { get; }
}