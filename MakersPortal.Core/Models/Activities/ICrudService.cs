namespace MakersPortal.Core.Models.Activities
{
    public interface ICrudService<TEntity> where TEntity : class
    {
        public void Add(TEntity entity);

        public void Update(TEntity entity);

        public void Delete(TEntity entity);

        public void GetAll(TEntity entity);
    }
}