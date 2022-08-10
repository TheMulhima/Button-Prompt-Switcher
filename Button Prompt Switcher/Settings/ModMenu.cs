namespace Button_Prompt_Switcher;

public static class ModMenu
{
    private static Menu MenuRef;

    private static GlobalSettings settings => Button_Prompt_Switcher.settings; 
    
    public static MenuScreen CreateMenuScreen(MenuScreen modListMenu)
    {
        MenuRef = new Menu("Mod Menu", new Element[]
        {
            CreateBoolOption(nameof(settings.NoCast), 
                "remove cast action from focus button/key"),

                new HorizontalOption(nameof(settings.ButtonSkinType), 
                "choose which button prompt type will be displayed for controllers (see readme)",
                new[] {"Default, 1", "2", "3", "4", "5", "6"},
                (i) =>
                {
                    if (i == 0)
                    {
                        settings.ButtonSkinTypeIsUsed = false;
                    }
                    else
                    {
                        settings.ButtonSkinTypeIsUsed = true;
                        settings.ButtonSkinType = i - 1;
                    }
                },
                () => settings.ButtonSkinType + 1),
            
            CreateBoolOption(nameof(settings.EmulatedXboxOnly), 
                "Forces game to use xbox 360 controller (see readme)"),
            
            CreateBoolOption(nameof(settings.SteamNintendoLayout),
                "Use Steam's nintendo controller layout")


        });

        return MenuRef.GetMenuScreen(modListMenu);
    }

    public static Element CreateBoolOption(string name, string desc)
    {
        return new HorizontalOption(name,
            desc,
            new[] { "False", "True" },
            (i) => ReflectionHelper.SetField(settings, name, i == 1),
            () => ReflectionHelper.GetField<GlobalSettings, bool>(settings, name) ? 1 : 0);
    }
}