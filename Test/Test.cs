using System.Numerics;
using ConsoleApp2;
using BankingLibrary.Services;

namespace Work2Tests
{
    [TestClass]
    public class PlayerTests
    {
        [TestMethod]
        public void TestBuyItem_SuccessfulPurchase()
        {
            // Arrange
            var player = new Player();
            int initialGold = player.Gold;
            var shop = new Shop();
            var item = shop.GetShopItems()[0]; // Предмет «Шлем»

            // Act
            player.BuyItem(item, shop);

            // Assert
            Assert.IsTrue(player.Inventory.Contains(item), "Предмет должен быть добавлен в инвентарь.");
            Assert.AreEqual(initialGold - item.Price + (player.Charisma / 2), player.Gold, "Золото игрока должно уменьшиться на стоимость предмета.");
        }

        [TestMethod]
        public void TestUpgradeStats_NotEnoughExperience()
        {
            // Arrange
            var player = new Player();
            player.Experience = 0; // Недостаточно опыта
            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            player.UpgradeStats();

            Assert.AreEqual(1, player.Level, "Уровень игрока не должен измениться.");
        }

        [TestMethod]
        public void TestUpgradeStats_IncreaseStrength()
        {
            // Arrange
            var player = new Player();
            // Устанавливаем ровно столько опыта, сколько нужно для повышения уровня (Level * 5)
            player.Experience = player.Level * 5;

            // Симулируем ввод «1» для повышения силы
            using var input = new StringReader("1" + Environment.NewLine);
            Console.SetIn(input);

            // Act
            player.UpgradeStats();

            // Assert
            Assert.AreEqual(2, player.Level, "Уровень должен увеличиться на 1.");
            Assert.AreEqual(6, player.Strength, "Сила должна увеличиться на 1.");
        }

        [TestMethod]
        public void TestGetEquippedItems()
        {
            // Arrange
            var player = new Player();
            // Помещаем тестовые предметы в слоты экипировки
            var headItem = new Item("Test Helmet", new Dictionary<string, int> { { "HP", 10 }, { "Armor", 2 } }, 20, EquipmentType.Head);
            player.Head = headItem;
            var torsoItem = new Item("Test Torso", new Dictionary<string, int> { { "HP", 10 }, { "Armor", 3 } }, 20, EquipmentType.Torso);
            player.Torso = torsoItem;

            // Act
            List<Item> equipped = player.GetEquippedItems();

            // Assert
            Assert.AreEqual(headItem, equipped[0], "Головной слот должен содержать тестовый предмет.");
            Assert.AreEqual(torsoItem, equipped[1], "Слот для брони на торс должен содержать тестовый предмет.");
            // Остальные слоты могут быть null
        }

        [TestMethod]
        public void TestGetInventory()
        {
            // Arrange
            var player = new Player();
            var item1 = new Item("Leather Pants", new Dictionary<string, int> { { "HP", 7 }, { "Armor", 2 } }, 15, EquipmentType.Legs); // для примера, хотя тип может быть любым
            var item2 = new Item("Traveler's Boots", new Dictionary<string, int> { { "HP", 3 }, { "Armor", 1 } }, 8, EquipmentType.Boots);
            player.Inventory.Add(item1);
            player.Inventory.Add(item2);

            // Act
            List<Item> inventory = player.GetInventory();

            // Assert
            CollectionAssert.Contains(inventory, item1);
            CollectionAssert.Contains(inventory, item2);
        }

        [TestMethod]
        public void TestEquipItem_Success()
        {
            // Arrange
            var player = new Player();
            // Добавляем предмет в инвентарь. Пусть это шлем, который добавляет +10 HP.
            var headItem = new Item("Test Helmet", new Dictionary<string, int> { { "HP", 10 }, { "Armor", 2 } }, 20, EquipmentType.Head);
            player.Inventory.Add(headItem);

            // Перед экипировкой у игрока HP рассчитывается как (HP + Strength) + AddedHP,
            // исходно AddedHP равен 0.
            int baseFullHP = player.HP + player.Strength;

            // Act
            player.EquipItem(headItem);

            // Assert
            Assert.AreEqual(headItem, player.Head, "Головной слот должен содержать экипированный предмет.");
            Assert.IsFalse(player.Inventory.Contains(headItem), "Экипированный предмет должен удаляться из инвентаря.");
            Assert.AreEqual(baseFullHP + 10, player.FullHP, "Общий HP должен увеличиться на значение из экипировки.");
        }

        [TestMethod]
        public void TestEquipItem_SlotOccupied()
        {
            // Arrange
            var player = new Player();
            var headItem1 = new Item("Leather Helmet", new Dictionary<string, int> { { "HP", 5 }, { "Armor", 1 } }, 10, EquipmentType.Head);
            var headItem2 = new Item("Test Helmet", new Dictionary<string, int> { { "HP", 10 }, { "Armor", 2 } }, 20, EquipmentType.Head);
            player.Inventory.Add(headItem1);
            player.Inventory.Add(headItem2);

            // Сначала экипируем первый предмет
            player.EquipItem(headItem1);

            // Act
            player.EquipItem(headItem2);

            // Второй предмет не должен быть экипирован и оставаться в инвентаре
            Assert.IsTrue(player.Inventory.Contains(headItem2));
        }

        [TestMethod]
        public void TestRest_RestoresHP()
        {
            // Arrange
            var player = new Player();
            // Установим текущие HP ниже максимально допустимых
            player.HP = 10;
            int maxHP = player.MaxHP;

            // Перед восстановлением, HP должно быть меньше maxHP
            Assert.IsTrue(player.HP < maxHP);

            // Перенаправляем вывод, чтобы подавить сообщение (опционально)
            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            player.Rest();

            // Assert
            Assert.AreEqual(maxHP, player.HP, "После отдыха здоровье должно быть полностью восстановлено.");
        }
    }
}
