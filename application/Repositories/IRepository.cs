namespace Application.Repositories
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Récupère tous les éléments de type T.
        /// </summary>
        Task<IReadOnlyList<T>> GetAllAsync();

        /// <summary>
        /// Récupère un élément par son identifiant.
        /// Retourne null si l'élément n'existe pas.
        /// </summary>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Ajoute un nouvel élément au contexte (ne sauvegarde pas automatiquement).
        /// </summary>
        Task AddAsync(T entity);

        /// <summary>
        /// Marque un élément comme modifié dans le contexte.
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Supprime un élément du contexte.
        /// </summary>
        void Delete(T entity);
    }
}