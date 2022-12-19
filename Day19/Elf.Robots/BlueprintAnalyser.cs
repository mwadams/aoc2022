namespace Elf.Robots;

using System;

public static class BlueprintAnalyser
{
    public static long AnalyseBlueprint(in Blueprint blueprint, int time)
    {
        RobotArmy army = new(1, 0, 0, 0);
        RobotArmy maxRobots = BuildMaxRobots(blueprint);
        ResourceStore store = new();
        int currentTime = 0;
        int maxGeodes = 0;

        Tick(time, blueprint, maxRobots, army, store, currentTime, ref maxGeodes);

        return maxGeodes;
    }

    public static Blueprint BuildBlueprint(ReadOnlySpan<char> line)
    {
        int colonIndex = line.IndexOf(':');
        int oreCostIndex = colonIndex + 23;
        int endOfOreCostIndex = line[oreCostIndex..].IndexOf(' ') + oreCostIndex;
        int clayOreCostIndex = endOfOreCostIndex + 28;
        int endOfClayOreCostIndex = line[clayOreCostIndex..].IndexOf(' ') + clayOreCostIndex;
        int obsidianOreCostIndex = endOfClayOreCostIndex + 32;
        int endOfObsidianOreCostIndex = line[obsidianOreCostIndex..].IndexOf(' ') + obsidianOreCostIndex;
        int obsidianClayCostIndex = endOfObsidianOreCostIndex + 9;
        int endOfObsidianClayCostIndex = line[obsidianClayCostIndex..].IndexOf(' ') + obsidianClayCostIndex;
        int geodeOreCostIndex = endOfObsidianClayCostIndex + 30;
        int endOfGeodeOreCostIndex = line[geodeOreCostIndex..].IndexOf(' ') + geodeOreCostIndex;
        int geodeObsidianCostIndex = endOfGeodeOreCostIndex + 9;
        int endOfGeodeObsidianCostIndex = line[geodeObsidianCostIndex..].IndexOf(' ') + geodeObsidianCostIndex;



        int oreOreCost = int.Parse(line[oreCostIndex..endOfOreCostIndex]);
        int clayOreCost = int.Parse(line[clayOreCostIndex..endOfClayOreCostIndex]);
        int obsidianOreCost = int.Parse(line[obsidianOreCostIndex..endOfObsidianOreCostIndex]);
        int obsidianClayCost = int.Parse(line[obsidianClayCostIndex..endOfObsidianClayCostIndex]);
        int geodeOreCost = int.Parse(line[geodeOreCostIndex..endOfGeodeOreCostIndex]);
        int geodeObsidianCost = int.Parse(line[geodeObsidianCostIndex..endOfGeodeObsidianCostIndex]);

        return new Blueprint(
            new RobotRecipe(oreOreCost, 0, 0),
            new RobotRecipe(clayOreCost, 0, 0),
            new RobotRecipe(obsidianOreCost, obsidianClayCost, 0),
            new RobotRecipe(geodeOreCost, 0, geodeObsidianCost)
            );
    }

    private static RobotArmy BuildMaxRobots(in Blueprint blueprint)
    {
        // You aren't going to need to build more robots than the maximum amount of ore you need to build the other robot types
        return new(
            Math.Max(blueprint.Ore.OreCost, Math.Max(blueprint.Clay.OreCost, Math.Max(blueprint.Obsidian.OreCost, blueprint.Geode.OreCost))),
            Math.Max(blueprint.Obsidian.ClayCost, blueprint.Geode.ClayCost),
            blueprint.Geode.ObsidianCost,
            int.MaxValue);

    }

    private static void Tick(int timeAvailable, in Blueprint blueprint, in RobotArmy maxRobots, in RobotArmy army, in ResourceStore store, int currentTime, ref int maxGeodes)
    {
        bool madeRecursiveCall = false;

        // Work through each robot type
        for (int robotTypeIndex = 0; robotTypeIndex < 4; robotTypeIndex++)
        {
            if (GetRobotCount(army, robotTypeIndex) == GetRobotCount(maxRobots, robotTypeIndex))
            {
                // Don't build this robot type if we already have as many as we need
                continue;
            }

            RobotRecipe recipe = GetRecipeForRobot(blueprint, robotTypeIndex);

            // Figure out how long we have to mine to get enough resources for another one of these robots
            int timeToMine = FindTimeToMine(recipe, store, army, timeAvailable);

            // Wind time forwards to the amount of mining time + the build time
            int newTime = currentTime + timeToMine + 1;
            if (newTime >= timeAvailable)
            {
                // We have run out of time to build this robot type, so give up, and move on to the next
                // robot type
                continue;
            }

            // Build all the robots we can in this elapsed time, creating new resource and robot store state
            // representing our situation after the amount of elaped time.
            BuildRobot(timeToMine, recipe, store, army, robotTypeIndex, out ResourceStore newStore, out RobotArmy newArmy);

            int remainingTime = timeAvailable - newTime;

            // If we were just to build geode robots and mine geodes every single turn from here on out,
            // how many would we end up with?
            // [This is the upper limit for how many geodes we could build if we kept going down this recursive path]
            // If it is less than the current maximum number of geodes one of our iterations has produced, then
            // give up now, because we aren't going to reach that number!
            if ((((remainingTime - 1) * remainingTime) / 2) // The number of geodes we will accumulate from the new geode robot we build every turn
                + newStore.Geode                            // The number of geodes we've already got
                + (remainingTime * newArmy.GeodeRobots)     // The number of geodes we can mine with our current army
                < maxGeodes)
            {
                continue;
            }

            // OK, we are going to go round again because we *may* be able to better than previous attempts
            madeRecursiveCall = true;
            Tick(timeAvailable, blueprint, maxRobots, newArmy, newStore, newTime, ref maxGeodes);
        }

        if (!madeRecursiveCall)
        {
            // Tell our caller whether we did a better job than our peers.
            maxGeodes = Math.Max(maxGeodes, store.Geode + (army.GeodeRobots * (timeAvailable - currentTime)));
        }
    }

    private static void BuildRobot(int timeToMine, in RobotRecipe recipe, in ResourceStore store, in RobotArmy army, int robotTypeIndex, out ResourceStore newStore, out RobotArmy newArmy)
    {
        // We've built a robot; the type we build is indicated by the robot type index.
        // First, add all the resources based on the existing robot army, and take away the resources used by the recipe to build the robot.
        int newOre = store.Ore + army.OreRobots * (timeToMine + 1) - recipe.OreCost;
        int newClay = store.Clay + army.ClayRobots * (timeToMine + 1) - recipe.ClayCost;
        int newObsidian = store.Obsidian + army.ObsidianRobots * (timeToMine + 1) - recipe.ObsidianCost;
        int newGeode = store.Geode + army.GeodeRobots * (timeToMine + 1);

        // Then figure out which robot we built
        int newGeodeRobots = army.GeodeRobots + (robotTypeIndex == 3 ? 1 : 0);
        int newObsidianRobots = army.ObsidianRobots + (robotTypeIndex == 2 ? 1 : 0);
        int newClayRobots = army.ClayRobots + (robotTypeIndex == 1 ? 1 : 0);
        int newOreRobots = army.OreRobots + (robotTypeIndex == 0 ? 1 : 0);

        // That (along with the new current time we have stashed earlier) is the new state after this chunk of time elapsed
        newStore = new ResourceStore(newOre, newClay, newObsidian, newGeode);
        newArmy = new RobotArmy(newOreRobots, newClayRobots, newObsidianRobots, newGeodeRobots);
    }

    private static int FindTimeToMine(in RobotRecipe recipe, in ResourceStore store, in RobotArmy army, int timeAvailable)
    {
        int minimumMiningTime = 0;
        // Foreach resource type in the recipe
        for (int i = 0; i < 3; ++i)
        {
            int recipeAmount = GetAmountForResourceType(recipe, i);
            int storeAmount = GetAmountForResourceType(store, i);

            if (recipeAmount == 0 || recipeAmount <= storeAmount)
            {
                // We don't have any constraint on this resource type; we have enough resources to build one
                continue;
            }

            int robotCount = GetRobotCount(army, i);

            if (robotCount == 0)
            {
                // We can't possibly mine enough while we don't have any robots for this resource type
                // This is essentially "wait forever" without risking an overflow.
                minimumMiningTime = timeAvailable + 1;
                continue;
            }

            // Without building any more robots, how long will it take to mine enough of this resource type
            int remainingTimeToMine = (recipeAmount - storeAmount + robotCount - 1) / robotCount;
            // Is this longer than amount of time we were going to try to mine other resources?
            minimumMiningTime = Math.Max(minimumMiningTime, remainingTimeToMine);
        }

        return minimumMiningTime;
    }

    private static int GetAmountForResourceType(in ResourceStore store, int resourceType)
    {
        return resourceType switch
        {
            0 => store.Ore,
            1 => store.Clay,
            2 => store.Obsidian,
            _ => throw new NotSupportedException()
        };
    }

    private static int GetAmountForResourceType(in RobotRecipe recipe, int resourceType)
    {
        return resourceType switch
        {
            0 => recipe.OreCost,
            1 => recipe.ClayCost,
            2 => recipe.ObsidianCost,
            _ => throw new NotSupportedException()
        };
    }

    private static RobotRecipe GetRecipeForRobot(in Blueprint blueprint, int robotTypeIndex)
    {
        return robotTypeIndex switch
        {
            0 => blueprint.Ore,
            1 => blueprint.Clay,
            2 => blueprint.Obsidian,
            3 => blueprint.Geode,
            _ => throw new NotSupportedException()
        };
    }

    private static int GetRobotCount(in RobotArmy army, int robotTypeIndex)
    {
        return robotTypeIndex switch
        {
            0 => army.OreRobots,
            1 => army.ClayRobots,
            2 => army.ObsidianRobots,
            3 => army.GeodeRobots,
            _ => throw new NotSupportedException()
        };
    }

}
