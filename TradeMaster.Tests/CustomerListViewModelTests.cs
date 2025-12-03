using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeMaster.Core.Entities;
using TradeMaster.Core.Interfaces;
using TradeMaster.Desktop.ViewModels;
using Xunit;

namespace TradeMaster.Tests
{
    public class CustomerListViewModelTests
    {
        [Fact]
        public async Task LoadCustomers_Populates_Customers_And_FilteredCustomers()
        {
            var seed = new List<Customer>
            {
                new Customer { Id = 1, Name = "Alice" },
                new Customer { Id = 2, Name = "Bob" }
            };
            var repo = new FakeRepository<Customer>(seed);
            // disable automatic background load in ctor
            var vm = new CustomerListViewModel(repo, autoLoad: false);

            // Trigger explicit load to avoid timing races from ctor background task
            await InvokeCommandAsync(vm, "LoadCustomersCommand");

            Assert.Equal(2, vm.Customers.Count);
            Assert.Equal(2, vm.FilteredCustomers.Count);
            Assert.Contains(vm.Customers, c => c.Name == "Alice");
            Assert.Contains(vm.Customers, c => c.Name == "Bob");
        }

        [Fact]
        public async Task SearchQuery_Filters_Customers_By_Name_Email_Phone()
        {
            var seed = new List<Customer>
            {
                new Customer { Id = 1, Name = "Alice", Email = "alice@example.com", Phone = "123" },
                new Customer { Id = 2, Name = "Bob", Email = "bob@example.com", Phone = "456" },
                new Customer { Id = 3, Name = "Alicia", Email = "alicia@sample.com", Phone = "789" }
            };
            var repo = new FakeRepository<Customer>(seed);
            var vm = new CustomerListViewModel(repo, autoLoad: false);

            await InvokeCommandAsync(vm, "LoadCustomersCommand");

            vm.SearchQuery = "ali";
            Assert.Equal(2, vm.FilteredCustomers.Count);
            Assert.Contains(vm.FilteredCustomers, c => c.Name == "Alice");
            Assert.Contains(vm.FilteredCustomers, c => c.Name == "Alicia");

            vm.SearchQuery = "bob@";
            Assert.Single(vm.FilteredCustomers);
            Assert.Equal("Bob", vm.FilteredCustomers.First().Name);

            vm.SearchQuery = "78";
            Assert.Single(vm.FilteredCustomers);
            Assert.Equal("Alicia", vm.FilteredCustomers.First().Name);
        }

        private static async Task InvokeCommandAsync(object viewModel, string commandPropertyName)
        {
            var prop = viewModel.GetType().GetProperty(commandPropertyName);
            if (prop == null) throw new InvalidOperationException($"Property '{commandPropertyName}' not found on {viewModel.GetType().FullName}.");

            var commandInstance = prop.GetValue(viewModel) ?? throw new InvalidOperationException("Command instance is null.");

            // Try ExecuteAsync(parameter) then Execute(parameter)
            var execAsync = commandInstance.GetType().GetMethod("ExecuteAsync", new[] { typeof(object) })
                            ?? commandInstance.GetType().GetMethod("ExecuteAsync", Type.EmptyTypes);
            if (execAsync != null)
            {
                var task = (Task?)execAsync.Invoke(commandInstance, execAsync.GetParameters().Length == 1 ? new object?[] { null } : Array.Empty<object?>());
                if (task != null) await task.ConfigureAwait(false);
                return;
            }

            var exec = commandInstance.GetType().GetMethod("Execute", new[] { typeof(object) })
                       ?? commandInstance.GetType().GetMethod("Execute", Type.EmptyTypes);
            if (exec != null)
            {
                exec.Invoke(commandInstance, exec.GetParameters().Length == 1 ? new object?[] { null } : Array.Empty<object?>());
                // allow small window for any background completion started by the command
                await Task.Delay(50);
                return;
            }

            throw new InvalidOperationException("No executable method found on command instance.");
        }

        private class FakeRepository<T> : IRepository<T> where T : class
        {
            private readonly List<T> _items;

            public FakeRepository(IEnumerable<T>? initial = null) => _items = initial?.ToList() ?? new List<T>();

            public Task AddAsync(T entity) { _items.Add(entity); return Task.CompletedTask; }

            public Task DeleteAsync(T entity) { _items.Remove(entity); return Task.CompletedTask; }

            public Task<IEnumerable<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
            {
                var compiled = predicate.Compile();
                return Task.FromResult<IEnumerable<T>>(_items.Where(compiled).ToList());
            }

            public Task<IEnumerable<T>> GetAllAsync() => Task.FromResult<IEnumerable<T>>(_items.ToList());

            public Task<T?> GetByIdAsync(int id)
            {
                var found = _items.FirstOrDefault(item =>
                {
                    var prop = item.GetType().GetProperty("Id");
                    if (prop == null) return false;
                    var value = prop.GetValue(item);
                    return value is int i && i == id;
                });
                return Task.FromResult(found);
            }

            public Task UpdateAsync(T entity)
            {
                var prop = entity.GetType().GetProperty("Id");
                if (prop != null)
                {
                    var id = (int?)prop.GetValue(entity);
                    if (id.HasValue)
                    {
                        var existing = _items.FirstOrDefault(item =>
                        {
                            var p = item.GetType().GetProperty("Id");
                            return p != null && (int?)p.GetValue(item) == id;
                        });
                        if (existing != null) { _items.Remove(existing); _items.Add(entity); }
                    }
                }
                return Task.CompletedTask;
            }
        }
    }
}