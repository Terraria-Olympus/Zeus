using System;
using System.Numerics;
using System.Threading;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Zeus.Engine;

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

            AsciiTitle.WriteTitle();

            Config = new();

            InitializeGraphics(debug);

            Console.WriteLine("Entering main loop.");
            Thread.Sleep(1000);
            Utils.HideConsole();
            Window.Visible = true;

            MainLoop();

            Utils.ShowConsole();
            Console.WriteLine();

            CleanUp();

            Console.Write("All resources cleaned up! Press any key to exit.");
            Console.ReadKey();
        }

        private static void InitializeGraphics(bool debug)
        {
            Console.Write("Creating window...");
            Utils.TryDo(() => {
                const int width = 400;
                const int height = 500;

                VeldridStartup.CreateWindowAndGraphicsDevice(
                    new WindowCreateInfo(50, 50, width, height, WindowState.Hidden, "Zeus"),
                    new GraphicsDeviceOptions(debug, null, true, ResourceBindingModel.Improved, true, true),
                    out Window,
                    out GraphicsDevice);
                
                Window.Resizable = false;
                Window.Visible = false;
                Window.Resized += () => {
                    GraphicsDevice.MainSwapchain.Resize((uint)Window.Width, (uint)Window.Height);
                    Controller.WindowResized(Window.Width, Window.Height);
                };
            });

            Console.Write("Creating command list...");
            Utils.TryDo(() => {
                CommandList = GraphicsDevice.ResourceFactory.CreateCommandList();
            });

            Console.Write("Creating ImGui controller...");
            Utils.TryDo(() => {
                Controller = new ImGuiController(GraphicsDevice, GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription, Window.Width, Window.Height);
            });
        }

        private static void MainLoop()
        {
            while (Window.Exists)
            {
                InputSnapshot snapshot = Window.PumpEvents();
                if (!Window.Exists) { break; }
                Controller.Update(1f / 60f, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

                UI.Draw();

                CommandList.Begin();
                CommandList.SetFramebuffer(GraphicsDevice.MainSwapchain.Framebuffer);
                CommandList.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 1f));
                Controller.Render(GraphicsDevice, CommandList);
                CommandList.End();
                GraphicsDevice.SubmitCommands(CommandList);
                GraphicsDevice.SwapBuffers(GraphicsDevice.MainSwapchain);
            }
        }

        private static void CleanUp()
        {
            Console.Write("Cleaning up graphics resources...");
            Utils.TryDo(() => {
                GraphicsDevice.WaitForIdle();

                if (Controller != null) Controller.Dispose();
                if (CommandList != null) CommandList.Dispose();
                if (GraphicsDevice != null) GraphicsDevice.Dispose();
            });

            if (EngineManager.Initialized)
            {
                Console.Write("Disposing engine...");
                Utils.TryDo(EngineManager.Dispose);
            }

            Console.Write("Saving config...");
            Utils.TryDo(Config.Save);
        }
    }
}
