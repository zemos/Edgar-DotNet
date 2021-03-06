﻿using System;
using System.Collections.Generic;
using System.Linq;
using Edgar.Geometry;
using Edgar.Legacy.Core.Doors.SimpleMode;
using Edgar.Legacy.Core.LayoutGenerators.DungeonGenerator;
using Edgar.Legacy.Core.MapDescriptions;
using Edgar.Legacy.Core.MapLayouts;
using Edgar.Legacy.GeneralAlgorithms.Algorithms.Common;
using Edgar.Legacy.GeneralAlgorithms.DataStructures.Polygons;
using Edgar.Legacy.Utils.Statistics;
using NUnit.Framework;

namespace MapGeneration.IntegrationTests.Utils.Statistics
{
    [TestFixture]
    public class EntropyCalculatorTests
    {
        private EntropyCalculator entropyCalculator;

        [SetUp]
        public void SetUp()
        {
            entropyCalculator = new EntropyCalculator();
        }

        [Test]
        public void GetRoomTemplatesDistribution_BasicTest()
        {
            var transformations = TransformationGrid2DHelper.GetAllTransformationsOld().ToList();
            var roomTemplate1 = new RoomTemplate(PolygonGrid2D.GetSquare(2), new SimpleDoorMode(1, 0), transformations);
            var roomTemplate2 = new RoomTemplate(PolygonGrid2D.GetSquare(4), new SimpleDoorMode(1, 0), transformations);
            var roomTemplate3 = new RoomTemplate(PolygonGrid2D.GetSquare(6), new SimpleDoorMode(1, 0), transformations);

            var data = new List<RoomTemplate>()
            {
                roomTemplate1,
                roomTemplate3,
                roomTemplate1,
                roomTemplate1
            };

            var availableRoomTemplates = new List<RoomTemplate>()
            {
                roomTemplate1,
                roomTemplate2,
                roomTemplate3,
            };

            var distribution = entropyCalculator.GetProbabilityDistribution(data, availableRoomTemplates);

            Assert.That(distribution.Count, Is.EqualTo(3));
            Assert.That(distribution[roomTemplate1], Is.EqualTo(3/4d));
            Assert.That(distribution[roomTemplate2], Is.EqualTo(0));
            Assert.That(distribution[roomTemplate3], Is.EqualTo(1/4d));
        }

        [Test]
        public void ComputeAverageRoomTemplatesEntropy_BasicTest()
        {
            var transformations = TransformationGrid2DHelper.GetAllTransformationsOld().ToList();
            var roomTemplate1 = new RoomTemplate(PolygonGrid2D.GetSquare(10), new SimpleDoorMode(1, 0), transformations);
            var roomTemplate2 = new RoomTemplate(PolygonGrid2D.GetRectangle(5, 10), new SimpleDoorMode(1, 0), transformations);

            var roomDescription1 = new BasicRoomDescription(new List<RoomTemplate>() { roomTemplate1, roomTemplate2 });

            var mapDescription = new MapDescription<int>();
            mapDescription.AddRoom(0, roomDescription1);
            mapDescription.AddRoom(1, roomDescription1);
            mapDescription.AddConnection(0, 1);

            var dungeonGenerator = new DungeonGenerator<int>(mapDescription);
            dungeonGenerator.InjectRandomGenerator(new Random(0));

            var layouts = new List<MapLayout<int>>();

            for (int i = 0; i < 10; i++)
            {
                layouts.Add(dungeonGenerator.GenerateLayout());
            }

            var entropy = entropyCalculator.ComputeAverageRoomTemplatesEntropy(mapDescription, layouts);

            Assert.That(entropy, Is.GreaterThanOrEqualTo(0));
            Assert.That(entropy, Is.LessThanOrEqualTo(1));
        }
    }
}