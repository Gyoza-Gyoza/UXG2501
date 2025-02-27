console.log("✅ script.js has been loaded successfully!");

document.addEventListener("DOMContentLoaded", function () {
    console.log("✅ DOM fully loaded. Waiting for Unity WebGL...");

    // Wait for Unity instance
    function notifyUnity() {
        console.log("📤 Sending message to Unity: HideRecycleBin");

        if (window.unityInstance) {
            window.unityInstance.SendMessage("WebGLInteraction", "ReceiveMessage", "HideRecycleBin");
            console.log("✅ Successfully sent message to Unity!");
        } else {
            console.warn("⚠️ Unity WebGL instance not ready yet.");
            setTimeout(notifyUnity, 500); // Retry every 500ms
        }
    }

    // Add click event listener to button
    const button = document.getElementById("hideRecycleBinButton");

    if (button) {
        button.addEventListener("click", function () {
            console.log("🖱️ Button clicked! Sending message to Unity.");
            notifyUnity();
        });
    } else {
        console.error("❌ Button element not found!");
    }
});