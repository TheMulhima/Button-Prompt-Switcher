namespace Button_Prompt_Switcher;

public static class ModMenu
{
    private static Menu MenuRef;

    public static MenuScreen CreateMenuScreen(MenuScreen modListMenu)
    {
        MenuRef = new Menu("Mod Menu", new Element[]
        {
            new HorizontalOption(nameof(Button_Prompt_Switcher.settings.NoCast), 
                "remove cast action from focus button/key",
                new []{"False", "True"},
                (i) => Button_Prompt_Switcher.settings.NoCast = i == 1,
                () => Button_Prompt_Switcher.settings.NoCast ? 1 : 0),
            
            new HorizontalOption(nameof(Button_Prompt_Switcher.settings.ButtonSkinType), 
                "choose which button prompt type will be displayed for controllers (see readme)",
                new[] {"Default, 1", "2", "3", "4", "5", "6"},
                (i) =>
                {
                    if (i == 0)
                    {
                        Button_Prompt_Switcher.settings.ButtonSkinTypeIsUsed = false;
                    }
                    else
                    {
                        Button_Prompt_Switcher.settings.ButtonSkinTypeIsUsed = true;
                        Button_Prompt_Switcher.settings.ButtonSkinType = i - 1;
                    }
                },
                () => Button_Prompt_Switcher.settings.ButtonSkinType + 1),
            
            new HorizontalOption(nameof(Button_Prompt_Switcher.settings.EmulatedXboxOnly), 
                "Forces game to use xbox 360 controller (see readme)", 
                new []{"False", "True"},
                (i) => Button_Prompt_Switcher.settings.EmulatedXboxOnly = i == 1,
                () => Button_Prompt_Switcher.settings.EmulatedXboxOnly ? 1 : 0)
        });

        return MenuRef.GetMenuScreen(modListMenu);
    }
}