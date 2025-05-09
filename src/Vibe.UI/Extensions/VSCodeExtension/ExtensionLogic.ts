import * as vscode from 'vscode';

export function activate(context: vscode.ExtensionContext) {
    console.log('Congratulations, your extension "vibe-ui-vscode-extension" is now active!');

    let disposable = vscode.commands.registerCommand('vibe-ui-vscode-extension.helloWorld', () => {
        vscode.window.showInformationMessage('Hello World from Vibe UI VSCode Extension!');
    });

    context.subscriptions.push(disposable);
}

export function deactivate() {}
