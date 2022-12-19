namespace Elf.Robots;
internal readonly ref struct ElfAccumulator
{
    private readonly string[] lines;

    public ElfAccumulator(string[] lines)
    {
        this.lines = lines;
    }

    public long Process()
    {
        Span<Blueprint> blueprints = stackalloc Blueprint[lines.Length];
        int blueprintCount = 0;
        foreach (var line in lines)
        {
            blueprints[blueprintCount++] = ProcessLine(line.AsSpan());
        }

        return AnalyseBlueprints(blueprints);
    }

    private static long AnalyseBlueprints(ReadOnlySpan<Blueprint> blueprints)
    {
        long total = 0; ;
        int blueprintIndex = 1;
        foreach (var blueprint in blueprints)
        {
            long result = AnalyseBlueprint(blueprint);
            total += result * blueprintIndex;
            blueprintIndex++;

        }

        return total;
    }

    private static long AnalyseBlueprint(in Blueprint blueprint)
    {
        RobotArmy army = new RobotArmy(1, 0, 0, 0);
        RobotArmy maxRobots = BuildMaxRobots(blueprint);
        ResourceStore store = new ResourceStore();
        int currentTime = 0;
        int maxGeodes = 0;

        Tick(24, blueprint, maxRobots, army, store, currentTime, ref maxGeodes);

        return maxGeodes;
    }

    private static RobotArmy BuildMaxRobots(Blueprint blueprint)
    {
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
                // Don't build this robot type if we have as many robots as the maxed out number of resources it takes to build
                // them
                continue;
            }

            RobotRecipe recipe = GetRecipeForRobot(blueprint, robotTypeIndex);
            int minimumMiningTime = FindMinimumMiningTime(recipe, store, army, timeAvailable);

            int newTime = currentTime + minimumMiningTime + 1;
            if (newTime >= timeAvailable)
            {
                continue;
            }

            BuildRobots(minimumMiningTime, recipe, store, army, robotTypeIndex, out ResourceStore newStore, out RobotArmy newArmy);

            int remainingTime = timeAvailable - newTime;

            // Greedily try to build geode robots for the remaining time
            if ((((remainingTime - 1) * remainingTime) / 2) // The number of iterations remaining
                + newStore.Geode                            // The number of geodes we've got
                + (remainingTime * newArmy.GeodeRobots)     // The number of geodes we can mine with our tobot
                < maxGeodes)
            {
                continue;
            }

            // OK, we are going to go round again because we can
            madeRecursiveCall = true;
            Tick(timeAvailable, blueprint, maxRobots, newArmy, newStore, newTime, ref maxGeodes);
        }

        if (!madeRecursiveCall)
        {
            // We ran out of time during this iteration, so that's the best we can do.
            maxGeodes = Math.Max(maxGeodes, store.Geode + (army.GeodeRobots * (timeAvailable - currentTime)));
        }
    }

    private static void BuildRobots(int elapsedTime, in RobotRecipe recipe, in ResourceStore store, in RobotArmy army, int robotTypeIndex, out ResourceStore newStore, out RobotArmy newArmy)
    {
        int newOre = store.Ore + army.OreRobots * (elapsedTime + 1) - recipe.OreCost;
        int newOreRobots = army.OreRobots + (robotTypeIndex == 0 ? 1 : 0);

        int newClay = store.Clay + army.ClayRobots * (elapsedTime + 1) - recipe.ClayCost;
        int newClayRobots = army.ClayRobots + (robotTypeIndex == 1 ? 1 : 0);

        int newObsidian = store.Obsidian + army.ObsidianRobots * (elapsedTime + 1) - recipe.ObsidianCost;
        int newObsidianRobots = army.ObsidianRobots + (robotTypeIndex == 2 ? 1 : 0);

        int newGeode = store.Geode + army.GeodeRobots * (elapsedTime + 1);
        int newGeodeRobots = army.GeodeRobots + (robotTypeIndex == 3 ? 1 : 0);

        newStore = new ResourceStore(newOre, newClay, newObsidian, newGeode);
        newArmy = new RobotArmy(newOreRobots, newClayRobots, newObsidianRobots, newGeodeRobots);
    }

    private static int FindMinimumMiningTime(in RobotRecipe recipe, in ResourceStore store, in RobotArmy army, int timeAvailable)
    {
        int minimumMiningTime = 0;
        // Foreach resource type in the recipe
        for (int i = 0; i < 3; ++i)
        {
            int recipeAmount = GetAmountForResourceType(recipe, i);
            int storeAmount = GetAmountForResourceType(store, i);
            if (recipeAmount == 0 || recipeAmount <= storeAmount)
            {
                // We don't have any constraint on this iteration; we have enough resources
                continue;
            }

            int robotCount = GetRobotCount(army, i);

            if (robotCount == 0)
            {
                // We can't possibly mine enough while we don't have any robots for this resource type
                minimumMiningTime = timeAvailable + 1;
            }
            else
            {
                // Without buildin any more robots, 
                int remainingToMine = recipeAmount - storeAmount + robotCount - 1;
                minimumMiningTime = Math.Max(minimumMiningTime, remainingToMine / robotCount);
            }
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

    private static Blueprint ProcessLine(ReadOnlySpan<char> line)
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
}