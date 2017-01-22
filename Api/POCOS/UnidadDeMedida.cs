namespace Api.Models.bfc
{
    /// <summary>
    /// 
    /// </summary>
    public class UnidadDeMedida
    {
        /// <summary>
        /// tb_codtab
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// tb_codstr
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// tb_destab
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Constructor vacio
        /// </summary>
        public UnidadDeMedida()
        {
        }        
    }
}