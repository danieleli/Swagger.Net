namespace Swagger.Net._Test.Custom.Extensions
{
    /// <summary>
    /// Foo Summary
    /// </summary>
    /// <remarks>Foo Remarks</remarks>
    public class Foo
    {
        /// <summary>
        /// Foo.FooName Summary
        /// </summary>
        /// <remarks>Foo.FooName Remarks</remarks>
        public string FooName { get; set; }

        /// <summary>
        /// Foo.GetFoo(int id) - Summary
        /// </summary>
        /// <remarks>Foo.GetFoo(int id) - Remarks</remarks>
        /// <param name="id" cref="int">id param comment</param>
        /// <returns cref="Foo">returns a foo by id</returns>
        public Foo GetFoo(int id)
        {
            return null;
        }
        
    }
}