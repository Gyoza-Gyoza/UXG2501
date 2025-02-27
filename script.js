console.log("✅ script.js has been loaded successfully!");

document.addEventListener("DOMContentLoaded", function () {
    console.log("✅ DOM fully loaded. Waiting for Unity WebGL...");

    function notifyUnity() {
        console.log("📤 Attempting to send message to Unity: HideRecycleBin");

        if (window.unityInstance) {
            window.unityInstance.SendMessage("WebGLInteractions", "OnMessageReceived", "HideRecycleBin");
            console.log("✅ Successfully sent message to Unity!");
        } else {
            console.warn("⚠️ Unity WebGL instance not ready. Retrying...");
            setTimeout(notifyUnity, 500);  // Retry every 500ms
        }
    }

    console.log("✅ Background observer script running...");

    const targetNode = document.getElementById("background-wallpaper");

    if (!targetNode) {
        console.error("❌ Target element #background-wallpaper not found!");
        return;
    }

    console.log("✅ Target Node found successfully!");

    let lastColor = window.getComputedStyle(targetNode).backgroundColor;

    setInterval(() => {
        let newColor = window.getComputedStyle(targetNode).backgroundColor;
        if (newColor !== lastColor) {
            console.log(`🎨 Background color changed! New color: ${newColor}`);
            lastColor = newColor;
            notifyUnity();
        }
    }, 500); // Check every 500ms
});
