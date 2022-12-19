namespace Elf.Robots
{
    public readonly record struct RobotArmy(int OreRobots, int ClayRobots, int ObsidianRobots, int GeodeRobots);

    public readonly record struct ResourceStore(int Ore, int Clay, int Obsidian, int Geode);

    public readonly record struct RobotRecipe(int OreCost, int ClayCost, int ObsidianCost);

    public readonly record struct Blueprint(in RobotRecipe Ore, in RobotRecipe Clay, in RobotRecipe Obsidian, in RobotRecipe Geode);
}
