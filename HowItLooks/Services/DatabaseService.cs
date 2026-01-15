using HowItLooks.Entities;
using HowItLooks.Helpers;
using HowItLooks.Models;
using SQLite;
using System.Linq;
using System.Linq.Expressions;

namespace HowItLooks.Services
{
    public class DatabaseService
    {
        private SQLiteConnection _db;

        public DatabaseService()
        {
            _db = DatabaseHelper.CreateDatabaseConnection();
            _db.CreateTable<GroupEntity>();
            _db.CreateTable<EnemyEntity>();
            _db.CreateTable<CampaignEntity>();
        }

        public List<EnemyEntity> GetAllMonsters()
        {
            return _db.Table<EnemyEntity>().ToList();
        }

        public List<EnemyEntity> GetAllMonstersBy(Expression<Func<EnemyEntity, bool>> predExpr)
        {
            return _db.Table<EnemyEntity>()
                .Where(predExpr).ToList();
        }

        public EnemyEntity GetMonsterById(int id)
        {
            return _db.Table<EnemyEntity>().FirstOrDefault(x =>  x.Id == id);
        }

        public EnemyEntity AddMonster(string name)
        {
            EnemyEntity enemy = new()
            {
                HitPointsLeft = 0,
                HitPoints = 0,
                Initiative = 0,
                Name = name
            };
            var res = _db.Insert(enemy);

            return enemy;
        }
        public EnemyEntity AddMonster(string name, int hitPoints, int armorClass, int? groupId = null)
        {
            EnemyEntity enemy = new()
            {
                HitPointsLeft = hitPoints,
                HitPoints = hitPoints,
                ArmorClass = armorClass,
                Initiative = 0,
                Name = name,
                GroupId = groupId
            };
            var res = _db.Insert(enemy);

            return enemy;
        }

        public void UpdateMonster(EnemyEntity enemy)
        {
            _db.Update(enemy);
        }

        public void DeleteMonster(EnemyEntity enemy)
        {
            _db.Delete(enemy);
        }

        public List<EnemyEntity> GetEnemiesByGroupId(int? groupId)
        {
            if (groupId == null)
                return _db.Table<EnemyEntity>().Where(x => x.GroupId == null).ToList();

            return _db.Table<EnemyEntity>().Where(x => x.GroupId == groupId).ToList();
        }

        public EnemyEntity AddEnemy(Enemy enemy, int? groupId = null)
        {
            var entity = new EnemyEntity(enemy)
            {
                GroupId = groupId
            };
            _db.Insert(entity);
            return entity;
        }

        public List<GroupEntity> GetAllGroups()
        {
            return _db.Table<GroupEntity>().ToList();
        }

        public GroupEntity? GetGroupByName(string name)
        {
            return _db.Table<GroupEntity>().FirstOrDefault(g => g.Name == name);
        }

        public GroupEntity? GetGroupById(int id)
        {
            return _db.Table<GroupEntity>().FirstOrDefault(g => g.Id == id);
        }

        public GroupEntity AddGroup(string name)
        {
            var group = new GroupEntity { Name = name };
            _db.Insert(group);
            return group;
        }

        public void DeleteGroup(GroupEntity group)
        {
            var enemies = _db.Table<EnemyEntity>().Where(e => e.GroupId == group.Id).ToList();
            foreach (var e in enemies)
                _db.Delete(e);

            _db.Delete(group);
        }

        public void UpdateGroup(GroupEntity group)
        {
            _db.Update(group);
        }

        public List<CampaignEntity> GetAllCampaigns()
        {
            return _db.Table<CampaignEntity>().ToList();
        }

        public CampaignEntity AddCampaign(string name)
        {
            var campaign = new CampaignEntity { Name = name };
            _db.Insert(campaign);
            return campaign;
        }

        public void UpdateCampaign(CampaignEntity campaign)
        {
            _db.Update(campaign);
        }

        public void DeleteCampaign(CampaignEntity campaign)
        {
            var enemies = _db.Table<EnemyEntity>().Where(e => e.CampaignId == campaign.Id).ToList();
            foreach (var e in enemies)
                _db.Delete(e);

            _db.Delete(campaign);
        }
        public CampaignEntity? GetCampaignByName(string name)
        {
            return _db.Table<CampaignEntity>().FirstOrDefault(c => c.Name == name);
        }
        public CampaignEntity? GetCampaignById(int id)
        {
            return _db.Table<CampaignEntity>().FirstOrDefault(c => c.Id == id);
        }
    }
}
