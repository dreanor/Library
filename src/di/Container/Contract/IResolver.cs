namespace DependencyInjection.Container.Contract
{
    internal interface IResolver
    {
        T Go<T>(IDiContainer diContainer);
    }
}