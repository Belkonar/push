using AutoMapper;
using data.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace data;


public static class DataExtensions
{
    /// <summary>
    /// This method handles the setting up of a mapping with appropriate ignores and reverse mapping
    /// </summary>
    /// <param name="expression">The automapper expression</param>
    /// <param name="setupReverse">whether or not to enable the reverse automatically</param>
    /// <typeparam name="TSrc">source type</typeparam>
    /// <typeparam name="TDest">dest type (must be a BaseDto)</typeparam>
    public static void Setup<TSrc, TDest>(this IMappingExpression<TSrc, TDest> expression, bool setupReverse = true) where TDest : BaseDto
    {
        expression = expression
            .ForMember(x => x.Created, opt => opt.Ignore())
            .ForMember(x => x.Updated, opt => opt.Ignore());

        if (setupReverse)
        {
            expression.ReverseMap();
        }
    }

    /// <summary>
    /// Mark an object with a specific state
    /// </summary>
    /// <param name="mainContext"></param>
    /// <param name="obj"></param>
    /// <param name="state"></param>
    /// <typeparam name="T"></typeparam>
    public static void Mark<T>(this MainContext mainContext, T obj, EntityState state = EntityState.Modified) where T : class
    {
        mainContext.Entry(obj).State = state;
    }
}