using System.Threading.Tasks;

namespace EchoForest.Core;

/// <summary>
/// Abstracts Godot's <c>SceneTree.ChangeSceneToFile</c> / <c>ResourceLoader</c>
/// behind a testable interface.
/// </summary>
public interface ISceneLoader
{
    /// <summary>
    /// Synchronously loads and switches to the scene at <paramref name="scenePath"/>.
    /// Throws <see cref="ArgumentNullException"/> if <paramref name="scenePath"/> is null.
    /// Throws <see cref="ArgumentException"/> if the path does not point to a valid scene resource.
    /// </summary>
    void LoadScene(string scenePath);

    /// <summary>
    /// Asynchronously loads the scene at <paramref name="scenePath"/> in the background.
    /// </summary>
    Task LoadSceneAsync(string scenePath);
}
