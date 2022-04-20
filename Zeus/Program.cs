using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Threading;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

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

            Console.WriteLine("Creating window...");
            int width = 400;
            int height = 500;

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

            Console.WriteLine("Creating command list...");
            CommandList = GraphicsDevice.ResourceFactory.CreateCommandList();

            Console.WriteLine("Creating ImGui controller...");
            Controller = new ImGuiController(GraphicsDevice, GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription, Window.Width, Window.Height);

            Console.WriteLine("Entering main loop...");
            Thread.Sleep(1000);
            Utils.HideConsole();
            Window.Visible = true;
            
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
            Utils.ShowConsole();
            Console.WriteLine();

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
    }
}
