using NUnit.Framework;
using OTS2026_PT1_GrupaA.Exceptions
using OTS2026_PT1_GrupaA.Models;
using OTS2026_PT1_GrupaA;
namespace OTS2026_PT1_GrupaA.Test
{
    public class GameTest
    {
        [Test]
        [TestFixture]
        public class GameInitializationTests
        {
            //equivalence partitioning
            // player i boat se nalaze u land zoni
            [Test]
            public void Constructor_PlayerAndBoatInLand_ShouldCreateGame()
            {
                Position playerPosition = new Position(10, 13);
                Position boatPosition = new Position(5, 5);

                Game game = new Game(playerPosition, boatPosition);

                Assert.That(game, Is.Not.Null);
                Assert.That(game.Player.Position.X, Is.EqualTo(10));
                Assert.That(game.Player.Position.Y, Is.EqualTo(13));
            }
            //player u pond zoni
            [Test]
            public void Constructor_PlayerInPond_ShouldThrowInvalidPlayerPositionException()
            {
                Position playerPosition = new Position(5, 20);
                Position boatPosition = new Position(5, 5);

                Assert.Throws<InvalidPlayerPositionException>(
                    () => new Game(playerPosition, boatPosition));
            }
            // boat u pond zoni
            [Test]
            public void Constructor_BoatInPond_ShouldThrowInvalidPlayerPositionException()
            {
                Position playerPosition = new Position(5, 5);
                Position boatPosition = new Position(5, 20);

                Assert.Throws<InvalidPlayerPositionException>(
                    () => new Game(playerPosition, boatPosition));
            }

            // player u invalid zoni
            [Test]
            public void Constructor_PlayerInInvalidZone_ShouldThrowInvalidPlayerPositionException()
            {
                Position playerPosition = new Position(29, 0);
                Position boatPosition = new Position(5, 5);

                Assert.Throws<InvalidPlayerPositionException>(
                    () => new Game(playerPosition, boatPosition));
            }

            // boat u invalid zoni
            [Test]
            public void Constructor_BoatInInvalidZone_ShouldThrowInvalidPlayerPositionException()
            {
                Position playerPosition = new Position(5, 5);
                Position boatPosition = new Position(29, 0);

                Assert.Throws<InvalidPlayerPositionException>(
                    () => new Game(playerPosition, boatPosition));
            }
            [TestFixture]


        }
        public class ValidatePositionTests
        {
            private Game game;
            //equivalence partitioning
            [SetUp]
            public void SetUp()
            {
                game = new Game(
                    new Position(10, 13),
                    new Position(5, 5));
            }
            [Test]
            public void ValidatePosition_NullPosition_ShouldReturnFalse()
            {
                bool result = game.ValidatePosition(null);

                Assert.That(result, Is.False);
            }
            [Test]
            [TestCase(0, 0)]
            [TestCase(24, 12)]
            [TestCase(10, 13)]
            [TestCase(19, 19)]
            [TestCase(5, 5)]
            public void ValidatePosition_LandZone_ShouldReturnTrue(int x, int y)
            {
                bool result = game.ValidatePosition(
                    new Position(x, y));

                Assert.That(result, Is.True);
            }
            [TestCase(0, 20)]
            [TestCase(25, 0)]
            [TestCase(20, 15)]
            [TestCase(9, 13)]
            public void ValidatePosition_InvalidZone_ShouldReturnFalse(
            int x,
            int y)
            {
                bool result = game.ValidatePosition(
                    new Position(x, y));

                Assert.That(result, Is.False);
            }
            [TestCase(5, 20)]
            [TestCase(29, 29)]
            [TestCase(15, 25)]
            public void ValidatePosition_PondWithoutBoat_ShouldReturnFalse(int x, int y)
            {
                game.Player.HasBoat = false;
                bool result = game.ValidatePosition(
                    new Position(x, y));

                Assert.That(result, Is.False);
            }
            [TestCase(5, 20)]
            [TestCase(29, 29)]
            [TestCase(15, 25)]
            public void ValidatePosition_PondWithBoat_ShouldReturnTrue(int x, int y)
            {
                game.Player.HasBoat = true;

                bool result = game.ValidatePosition(
                    new Position(x, y));
                Assert.That(result, Is.True);
            }
            [TestCase(-1, 0)]
            [TestCase(0, -1)]
            [TestCase(30, 0)]
            [TestCase(0, 30)]
            [TestCase(30, 30)]
            //boundary value analysis
            public void ValidatePosition_OutsideMap_ShouldReturnFalse(int x, int y)
            {
                bool result = game.ValidatePosition(
                    new Position(x, y));

                Assert.That(result, Is.False);
            }
        }
        public class ResolvePlayerPositionTests
        {
            //equivalence partitioning
            [SetUp]
            public void SetUp()
            {
                game = new Game(
                    new Position(10, 13),
                    new Position(5, 5));
            }
            // polje sadrzi bait
            [Test]
            public void ResolvePlayerPosition_Bait_ShouldIncreaseAmountOfBait()
            {
                Position position = game.Player.Position;

                game.Map.AddContentToFieldOnPosition(
                    FieldContent.Bait,
                    position);

                int baitBefore = game.Player.AmountOfBait;

                game.ResolvePlayerPosition();

                Assert.Multiple(() =>
                {
                    Assert.That(
                        game.Player.AmountOfBait,
                        Is.EqualTo(baitBefore + 1));

                    Assert.That(
                        game.Map.Fields[position.X, position.Y].Content,
                        Is.EqualTo(FieldContent.Empty));
                });
            }
            // polje sadrzi boat
            [Test]
            public void ResolvePlayerPosition_Boat_ShouldSetHasBoatToTrue()
            {
                Position position = game.Player.Position;

                game.Map.AddContentToFieldOnPosition(
                    FieldContent.Boat,
                    position);

                game.ResolvePlayerPosition();

                Assert.Multiple(() =>
                {
                    Assert.That(game.Player.HasBoat, Is.True);

                    Assert.That(
                        game.Map.Fields[position.X, position.Y].Content,
                        Is.EqualTo(FieldContent.Empty));
                });
            }
            // fish i ima mamac
            [Test]
            public void ResolvePlayerPosition_FishWithBait_ShouldCatchFish()
            {
                Position pondPosition = new Position(5, 20);

                game.Player.Position = pondPosition;
                game.Player.AmountOfBait = 3;

                game.Map.AddContentToFieldOnPosition(
                    FieldContent.Fish,
                    pondPosition);

                int fishBefore = game.Player.AmountOfFish;

                game.ResolvePlayerPosition();

                Assert.Multiple(() =>
                {
                    Assert.That(
                        game.Player.AmountOfFish,
                        Is.EqualTo(fishBefore + 1));

                    Assert.That(
                        game.Player.AmountOfBait,
                        Is.EqualTo(2));

                    Assert.That(
                        game.Map.Fields[pondPosition.X, pondPosition.Y].Content,
                        Is.EqualTo(FieldContent.Empty));
                });
            }
            // fish i nema mamac
            [Test]
            public void ResolvePlayerPosition_FishWithoutBait_ShouldNotCatchFish()
            {
                Position pondPosition = new Position(5, 20);

                game.Player.Position = pondPosition;
                game.Player.AmountOfBait = 0;

                game.Map.AddContentToFieldOnPosition(FieldContent.Fish, pondPosition);

                game.ResolvePlayerPosition();

                Assert.Multiple(() =>
                {
                    Assert.That(
                        game.Player.AmountOfFish,
                        Is.EqualTo(0));

                    Assert.That(
                        game.Player.AmountOfBait,
                        Is.EqualTo(0));

                    Assert.That(
                        game.Map.Fields[pondPosition.X, pondPosition.Y].Content,
                        Is.EqualTo(FieldContent.Empty));
                });
            }
            //prazno polje
            [Test]
            public void ResolvePlayerPosition_EmptyField_ShouldNotChangePlayerState()
            {
                Position position = game.Player.Position;

                game.Player.AmountOfBait = 5;
                game.Player.AmountOfFish = 4;
                game.Player.HasBoat = true;

                game.ResolvePlayerPosition();

                Assert.Multiple(() =>
                {
                    Assert.That(game.Player.AmountOfBait, Is.EqualTo(5));
                    Assert.That(game.Player.AmountOfFish, Is.EqualTo(4));
                    Assert.That(game.Player.HasBoat, Is.True);

                    Assert.That(
                        game.Map.Fields[position.X, position.Y].Content,
                        Is.EqualTo(FieldContent.Empty));
                });
            }
        }


    }

