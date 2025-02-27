console.log("✅ script.js has been loaded successfully!");

document.addEventListener("DOMContentLoaded", function () {

    function notifyUnity() {
        console.log("📤 Sending message to Unity: HideRecycleBin");

        if (window.unityInstance) {
            window.unityInstance.SendMessage("WebGLInteraction", "ReceiveMessage", "HideRecycleBin");
            console.log("✅ Successfully sent message to Unity!");
        }
    }

    //Add click event listener to button
    const button = document.getElementById("hideRecycleBinButton");

    if (button) {
        button.addEventListener("click", function () {
            console.log("🖱️ Button clicked! Sending message to Unity.");
            notifyUnity();
        });
    }

    console.log("✅ Starting MutationObserver for comment tracking...");

    let commentNode = null;
    document.body.childNodes.forEach((node) => {
        if (node.nodeType === Node.COMMENT_NODE && node.nodeValue.includes("Background Color")) {
            commentNode = node;
        }
    });

    //Observe changes to the comment node
    const observer = new MutationObserver(() => {
        console.log(`📝 Comment changed: ${commentNode.nodeValue}`);
        notifyUnity();
    });

    observer.observe(commentNode, { characterData: true, subtree: true });

    console.log("✅ MutationObserver is monitoring comment changes...");
});
