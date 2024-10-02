module DesignerContext

open Feliz
open System

// TODO: Use proper authentication

type AuthContextType =
    {
        User : Types.User option
        SignIn : string -> (unit -> unit) -> unit
        SignOut : (unit -> unit) -> unit
    }

let DesignerContext = React.createContext<AuthContextType option>("DesignerContext", None)

[<Hook>]
let useDesigner () =
    React.useContext(DesignerContext)

[<ReactComponent>]
let DesignerProvider
    ( props :
        {|
            children : ReactElement
        |}
    ) =

    let (user, setUser) = React.useState<Types.User option>(None)

    let signIn (newUser : string) (callBack : unit -> unit) =
        let user =
            {
                Id = Guid.NewGuid()
                Login = newUser
            } : Types.User

        setUser(Some user)
        callBack()

    let signOut (callBack : unit -> unit) =
        setUser(None)
        callBack()

    let value =
        {
            User = user
            SignIn = signIn
            SignOut = signOut
        }

    React.contextProvider(
        DesignerContext,
        Some value,
        props.children
    )
