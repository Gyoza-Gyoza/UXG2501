console.log("✅ script.js has been loaded successfully!");

    //----------------------------------- Change Images (Light to Dark) -----------------------------------------------
    function ChangeWebBg() {
        changeTableImage();
        changeOverlayImage();
        changePostItImage();
    }

    function changeTableImage() {
        const imageContainer = document.getElementById("table-background"); //Ensure this ID exists in your HTML
        if (imageContainer) {
            imageContainer.src = "Build/Images/Table Dark.png"; //Change the image dynamically
            console.log("✅ Image updated via MutationObserver.");
        } else {
            console.error("❌ Image container not found!");
        }
    }

    function changeOverlayImage() {
        const imageContainer = document.getElementById("overlay-image"); //Ensure this ID exists in your HTML
        if (imageContainer) {
            imageContainer.src = "Build/Images/Monitor Dark.png"; //Change the image dynamically
            console.log("✅ Image updated via MutationObserver.");
        } else {
            console.error("❌ Image container not found!");
        }
    }

    function changePostItImage() {
        const imageContainer = document.getElementById("postit-note-1"); //Ensure this ID exists in your HTML
        if (imageContainer) {
            imageContainer.src = "Build/Images/Layer 2 Dark.png"; //Change the image dynamically
            console.log("✅ Image updated via MutationObserver.");
        } else {
            console.error("❌ Image container not found!");
        }
    }

document.addEventListener("DOMContentLoaded", function () {

    //----------------------------------- Send Unity Message -----------------------------------------------
    function notifyUnityRecyleBin() {
        console.log("📤 Sending message to Unity: HideRecycleBin");

        if (window.unityInstance) {
            window.unityInstance.SendMessage("Systems", "ReceiveMessage", "HideRecycleBin");
            console.log("✅ Successfully sent message to Unity!");
        }
    }

    function notifyUnitySmartHome() {
        console.log("📤 Sending message to Unity: SmartHome");

        if (window.unityInstance) {
            window.unityInstance.SendMessage("Systems", "ReceiveMessage", "SmartHome");
            console.log("✅ Successfully sent message to Unity!");
        }
    }

    function notifyUnityRoot() {
        console.log("📤 Sending message to Unity: BlackScreen");

        if (window.unityInstance) {
            window.unityInstance.SendMessage("Systems", "ReceiveMessage", "BlackScreen");
            console.log("✅ Successfully sent message to Unity!");
        }
    }

    //--------------------------------------------------------------------------------------------------

    // ✅ Add click event listener to button (Not Used)
    // const button = document.getElementById("hideRecycleBinButton");
    // if (button) {
    //     button.addEventListener("click", function () {
    //         console.log("🖱️ Button clicked! Sending message to Unity.");
    //         notifyUnity();
    //     });
    // }

    //----------------------------------- Monitor Comment Changes for Phase 1 Edit -----------------------------------------------

    console.log("✅ Starting MutationObserver for ChatAPT comment tracking...");
    let commentNode = null;
    document.body.childNodes.forEach((node) => {
        if (node.nodeType === Node.COMMENT_NODE && node.nodeValue.includes("CHATAPT.exe_permission_level")) {
            commentNode = node;
        }
    });

    if (commentNode) {
        const observer = new MutationObserver(() => {
            console.log(`📝 Comment changed: ${commentNode.nodeValue}`);
            notifyUnityRecyleBin();
        });
        observer.observe(commentNode, { characterData: true, subtree: true });

        // ✅ Ensure updates even if tab is inactive
        setInterval(() => {
            if (document.hidden) {
                observer.takeRecords();
            }
        }, 500);
    } else {
        console.error("❌ Comment node not found!");
    }

     //----------------------------------- Monitor Background Color Changes for Phase 2 Edit -----------------------------------------------

    console.log("✅ Starting MutationObserver for background color...");
    const targetNode = document.getElementById("smart-home-control-panel");

    if (!targetNode) {
        console.error("❌ Target element #smart-home-control-panel not found!");
        return;
    }

    console.log("✅ Target Node found successfully!");
    let lastColor = window.getComputedStyle(targetNode).backgroundColor;

    // ✅ MutationObserver for inline style changes
    const bgObserver = new MutationObserver(() => {
        let newColor = window.getComputedStyle(targetNode).backgroundColor;
        if (newColor !== lastColor) {
            console.log(`🎨 Background color changed! New color: ${newColor}`);
            lastColor = newColor;
            changeTableImage();
            changeOverlayImage();
            changePostItImage();
            //notifyUnitySmartHome();
        }
    });

    bgObserver.observe(targetNode, { attributes: true, attributeFilter: ["style"] });

    console.log("✅ MutationObserver is monitoring background color changes...");

    // ✅ Fallback polling method (detects computed styles, even if changed via CSS)
    setInterval(() => {
        let newColor = window.getComputedStyle(targetNode).backgroundColor;
        if (newColor !== lastColor) {
            console.log(`🎨 Background color changed via computed style! New color: ${newColor}`);
            lastColor = newColor;
            changeTableImage();
            changeOverlayImage();
            changePostItImage();
        }
    }, 500);

    //--------------------------------------------------------------------------------------------------

    //----------------------------------- Monitor Comment Changes for Phase 2 Edit -----------------------------------------------

    console.log("✅ Starting MutationObserver for Root Access comment tracking...");
    let commentNodeRoot = null;
    document.body.childNodes.forEach((node) => {
        if (node.nodeType === Node.COMMENT_NODE && node.nodeValue.includes("CHATAPT.exe_root_write_access")) {
            commentNodeRoot = node;
        }
    });

    if (commentNodeRoot) {
        const observer = new MutationObserver(() => {
            console.log(`📝 Comment changed: ${commentNodeRoot.nodeValue}`);
            notifyUnityRoot();
        });
        observer.observe(commentNodeRoot, { characterData: true, subtree: true });

        // ✅ Ensure updates even if tab is inactive
        setInterval(() => {
            if (document.hidden) {
                observer.takeRecords();
            }
        }, 500);
    } else {
        console.error("❌ Comment node not found!");
    }
});
