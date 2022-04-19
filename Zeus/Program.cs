using ImGuiNET;
using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Zeus.Engine;
using static ImGuiNET.ImGuiNative;

namespace Zeus
{
    public static class Program
    {
        public static Sdl2Window Window;
        public static GraphicsDevice GraphicsDevice;
        public static CommandList CommandList;
        public static ImGuiController Controller;

        public static ConfigManager Config;

        private static Vector3 _clearColor = new(0f, 0f, 0f);

        public static void Main(string[] args)
        {
            bool debug = false;
#if DEBUG
            debug = true;
#endif

            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;

            AsciiTitle.WriteTitle();

            Config = new();

            Console.WriteLine("Creating window...");
            int width = 400;
            int height = 500;
            
            VeldridStartup.CreateWindowAndGraphicsDevice(
                new WindowCreateInfo(50, 50, width, height, WindowState.Normal, "Zeus"),
                new GraphicsDeviceOptions(debug, null, true, ResourceBindingModel.Improved, true, true),
                out Window,
                out GraphicsDevice);
            Window.Resizable = false;
            Window.Resized += () => {
                GraphicsDevice.MainSwapchain.Resize((uint)Window.Width, (uint)Window.Height);
                Controller.WindowResized(Window.Width, Window.Height);
            };

            Console.WriteLine("Creating command list...");
            CommandList = GraphicsDevice.ResourceFactory.CreateCommandList();

            Console.WriteLine("Creating ImGui controller...");
            Controller = new ImGuiController(GraphicsDevice, GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription, Window.Width, Window.Height);

            Console.WriteLine("Entering main loop.");
            while (Window.Exists)
            {
                InputSnapshot snapshot = Window.PumpEvents();
                if (!Window.Exists) { break; }
                Controller.Update(1f / 60f, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

                SubmitZeusUI();

                CommandList.Begin();
                CommandList.SetFramebuffer(GraphicsDevice.MainSwapchain.Framebuffer);
                CommandList.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 1f));
                Controller.Render(GraphicsDevice, CommandList);
                CommandList.End();
                GraphicsDevice.SubmitCommands(CommandList);
                GraphicsDevice.SwapBuffers(GraphicsDevice.MainSwapchain);
            }

            Console.WriteLine("Window closed! Cleaning up Veldrid resources...");
            GraphicsDevice.WaitForIdle();
            Controller.Dispose();
            CommandList.Dispose();
            GraphicsDevice.Dispose();

            Console.WriteLine("Saving config...");
            Config.Save();

            Console.WriteLine("Veldrid resources cleaned up! Press any key to exit.");
            Console.ReadKey();
        }

        private static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Console.WriteLine("Resovling " + args.Name);
            
            string path = Path.Combine(Environment.CurrentDirectory, "libs", args.Name + ".dll");

            if (File.Exists(path))
                return Assembly.LoadFrom(path);

            throw new Exception("Could not resolve " + args.Name);
        }

        private static unsafe void SubmitZeusUI()
        {
            switch (Config.Data.Theme) {
                case "Dark": ImGui.StyleColorsDark(); break;
                case "Light": ImGui.StyleColorsLight(); break;
                default:  ImGui.StyleColorsDark(); break;
            }
            
            ImGui.SetNextWindowPos(new(0, 0));
            ImGui.SetNextWindowSize(new(Window.Width, 400));
            ImGui.Begin("Item Checklist", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse);
            
            ImGui.End();

            ImGui.SetNextWindowPos(new(0, 400));
            ImGui.SetNextWindowSize(new(Window.Width, 100));
            ImGui.Begin("Settings", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse);
            
            ImGui.Text("Player to track: ");
            ImGui.SameLine();
            ImGui.InputText("", ref EngineManager.CurrentlyTrackedPlayerName, 32);

            ImGui.Text("Theme:");
            ImGui.SameLine();
            if (ImGui.RadioButton("Dark", Config.Data.Theme == "Dark")) Config.Data.Theme = "Dark";
            ImGui.SameLine();
            if (ImGui.RadioButton("Light", Config.Data.Theme == "Light")) Config.Data.Theme = "Light";

            if (!EngineManager.Initialized)
            {
                if (ImGui.Button("Initialize Mode A."))
                    ;

                ImGui.SameLine();
                if (ImGui.Button("Initialize Mode B."))
                    ;
            }

            ImGui.End();
        }
    }
}
