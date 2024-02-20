using System;
using System.Collections.Generic;
using System.Linq;

namespace Colosoft.Presentation.Themes
{
    public class AssemblyResourcesManager : IAssemblyResourcesManager
    {
        private readonly Dictionary<string, Lazy<System.Reflection.Assembly>> assemblies = new Dictionary<string, Lazy<System.Reflection.Assembly>>();
        private readonly AssemblyResourcesRepository assemblyResourcesRepository;
        private readonly Lazy<Reflection.IAssemblyRepository> assemblyRepository;
        private readonly Lazy<Reflection.IAssemblyInfoRepository> assemblyInfoRepository;
        private bool initialized;

        public virtual IResourcesRepository AssemblyResourcesRepository => this.assemblyResourcesRepository;

        public AssemblyResourcesManager(
            Lazy<Reflection.IAssemblyInfoRepository> assemblyInfoRepository,
            Lazy<Reflection.IAssemblyRepository> assemblyRepository)
        {
            this.assemblyInfoRepository = assemblyInfoRepository;
            this.assemblyRepository = assemblyRepository;
            this.assemblyResourcesRepository = new AssemblyResourcesRepository(this);
        }

        protected virtual void OnFailLoadAssembly(System.Reflection.AssemblyName name, Exception exception)
        {
        }

        protected void CheckInitialize()
        {
            if (!this.initialized)
            {
                this.initialized = true;

                var directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                foreach (var i in System.IO.Directory.GetFiles(directory, "*.dll")
                    .Union(System.IO.Directory.GetFiles(directory, "*.exe")))
                {
                    try
                    {
                        var assemblyName = System.Reflection.AssemblyName.GetAssemblyName(i);

                        if (!this.assemblies.ContainsKey(assemblyName.Name))
                        {
                            this.assemblies.Add(assemblyName.Name, new Lazy<System.Reflection.Assembly>(
                                () => System.Reflection.Assembly.Load(assemblyName)));
                        }
                    }
                    catch
                    {
                        // ignore
                    }
                }
            }
        }

        public virtual void AddDirectory(string directoryName)
        {
            if (string.IsNullOrWhiteSpace(directoryName))
            {
                throw new ArgumentException($"'{nameof(directoryName)}' cannot be null or whitespace.", nameof(directoryName));
            }

            if (System.IO.Directory.Exists(directoryName))
            {
                foreach (var i in System.IO.Directory.GetFiles(directoryName, "*.dll")
                    .Union(System.IO.Directory.GetFiles(directoryName, "*.exe")))
                {
                    try
                    {
                        var assemblyName = System.Reflection.AssemblyName.GetAssemblyName(i);

                        if (!this.assemblies.ContainsKey(assemblyName.Name))
                        {
#pragma warning disable S3885 // "Assembly.Load" should be used
                            this.assemblies.Add(
                                assemblyName.Name,
                                new Lazy<System.Reflection.Assembly>(() => System.Reflection.Assembly.LoadFile(i)));
#pragma warning restore S3885 // "Assembly.Load" should be used
                        }
                    }
                    catch
                    {
                        // ignore
                    }
                }
            }
        }

        public virtual void AddFile(string assemblyFileName)
        {
            if (string.IsNullOrWhiteSpace(assemblyFileName))
            {
                throw new ArgumentException($"'{nameof(assemblyFileName)}' cannot be null or whitespace.", nameof(assemblyFileName));
            }

            var assemblyName = System.Reflection.AssemblyName.GetAssemblyName(assemblyFileName);

            if (!this.assemblies.ContainsKey(assemblyName.Name))
            {
#pragma warning disable S3885 // "Assembly.Load" should be used
                this.assemblies.Add(
                    assemblyName.Name,
                    new Lazy<System.Reflection.Assembly>(() => System.Reflection.Assembly.LoadFile(assemblyFileName)));
#pragma warning restore S3885 // "Assembly.Load" should be used
            }
        }

        public virtual System.Reflection.Assembly Get(string assemblyName)
        {
            this.CheckInitialize();

            Lazy<System.Reflection.Assembly> result = null;

            if (this.assemblies.TryGetValue(assemblyName, out result))
            {
                return result.Value;
            }

            try
            {
                var asm = AppDomain.CurrentDomain.Load(assemblyName);

                if (asm != null)
                {
                    var name = Reflection.AssemblyExtensions.GetAssemblyNameWithoutExtension(assemblyName);

                    if (!this.assemblies.ContainsKey(name))
                    {
                        this.assemblies.Add(name, new Lazy<System.Reflection.Assembly>(() => asm));
                    }

                    return asm;
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                // Quanto o arquivo do assembly não foi encontrado
            }
            catch (System.IO.FileLoadException)
            {
                // Quando não foi possível carregar o arquivo do assembly
            }
            catch (BadImageFormatException)
            {
                // Quando da biblioteca que está tentando se carregada não é válida
            }
            catch
            {
                // ignore
            }

            Reflection.AssemblyInfo assemblyInfo = null;

            try
            {
                Exception lastException = null;
                this.assemblyInfoRepository.Value.TryGet(assemblyName, out assemblyInfo, out lastException);

                if (lastException != null)
                {
                    throw lastException;
                }
            }
            catch (Exception ex)
            {
                throw new AssemblyResourcesException(
                    ResourceMessageFormatter.Create(() => Properties.Resources.AssemblyResourcesManager_FailOnGetAssemblyInfo, assemblyName).Format(), ex);
            }

            if (assemblyInfo != null)
            {
                var packagesContainer = this.assemblyRepository.Value.GetAssemblyPackages(
                    new Reflection.AssemblyPart[]
                    {
                        new Reflection.AssemblyPart(string.Concat(assemblyName, ".dll")),
                    });

                if (packagesContainer != null)
                {
                    foreach (var package in packagesContainer)
                    {
                        foreach (var name in package)
                        {
                            Exception assemblyLoadException = null;

                            if (!this.assemblies.ContainsKey(name.Source))
                            {
                                var asm = package.LoadAssemblyGuarded(name, out assemblyLoadException);

                                var asmName = System.IO.Path.GetFileNameWithoutExtension(name.Source);

                                if (asm != null && !this.assemblies.ContainsKey(asmName))
                                {
                                    this.assemblies.Add(asmName, new Lazy<System.Reflection.Assembly>(() => asm));
                                }
                                else
                                {
                                    this.OnFailLoadAssembly(new System.Reflection.AssemblyName(name.Source), assemblyLoadException);
                                }
                            }
                        }
                    }
                }
            }

            if (!this.assemblies.TryGetValue(assemblyName, out result))
            {
                throw new AssemblyResourcesException(
                  ResourceMessageFormatter.Create(() => Properties.Resources.AssemblyResourcesManager_AssemblyNotFound, assemblyName).Format());
            }

            return result.Value;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.assemblyResourcesRepository.Dispose();
        }
    }
}
