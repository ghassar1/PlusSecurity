namespace AcSys.Core.Data.Model.Base
{
    public abstract class NamedEntityBase : EntityBase, INamedEntity
    {
        public string Name { get; set; }

        public override void Validate()
        {
        }

        public override string ToDescription()
        {
            //return base.ToDescription();
            return this.Name;
        }
    }
}
