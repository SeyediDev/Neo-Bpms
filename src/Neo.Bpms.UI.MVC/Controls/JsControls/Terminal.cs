namespace Neo.Bpms.UI.MVC.Controls.JsControls;

public class Terminal : CabdoJsControl
{
    public Terminal(InputFieldDefinition field, ControlsRendererData controlsRenderer) : base(field,
        controlsRenderer)
    {
    }

    public override NeoStringBuilder Render()
    {
        string contentValue = ControlsRendererData.Record.GetString(Field.FieldName);
        NeoStringBuilder result = new(); // todo calculate cols and rows
        result +=
            $@"
            const ansiEscapeRegex = new RegExp(
                ['[\u001b\u009b][[()#;?]*(?:[0-9]{1,4}',
                '(?:;[0-9]{0,4})*)?[0-9A-ORZcf-nqry=><]'].join(''), 'g'
            );
            const terminal = new Terminal({{
                cursorBlink: true,
                cols: 120,
                rows: 40
            }});
            terminal.open(document.getElementById(""{Field.FieldName}""));            
            terminal.write({JsonConvert.ToString(contentValue)}.replace(ansiEscapeRegex, ''));
                        ";
        //             result += $@"
        // const txtArea = $('<textarea/>');
        // txtArea.val({JsonConvert.ToString(contentValue)});
        // $('#main-part').append(txtArea)";
        return result;
    }
}