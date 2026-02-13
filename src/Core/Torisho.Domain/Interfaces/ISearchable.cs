using System.Collections.Generic;

namespace Torisho.Domain.Interfaces;

public interface ISearchable
{
    IEnumerable<object> Search(string keyword);
    IEnumerable<object> Filter(IDictionary<string, object> criteria);
}
